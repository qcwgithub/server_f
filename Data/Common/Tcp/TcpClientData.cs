using System.Net.Sockets;
using System.Net;

namespace Data
{
    public partial class TcpClientData : ProtocolClientData
    {
        Socket socket;
        public readonly bool forClient;

        ////
        int closing;
        public bool IsClosing()
        {
            return Volatile.Read(ref this.closing) == 1;
        }

        int cleanuped;
        public override bool IsClosed()
        {
            return Volatile.Read(ref this.closing) == 1;
        }

        public override EndPoint RemoteEndPoint => this.socket.RemoteEndPoint;

        int ioRef;
        public bool TryIncreaseIORef()
        {
            if (this.IsClosing())
            {
                return false;
            }

            Interlocked.Increment(ref this.ioRef);

            if (this.IsClosing())
            {
                Interlocked.Decrement(ref this.ioRef);
                return false;
            }

            return true;
        }
        public int DecreaseIORef()
        {
            return Interlocked.Decrement(ref this.ioRef);
        }

        SendPart sendPart;
        RecvPart recvPart;

        // Connector
        public TcpClientData(IProtocolClientCallback callback, string ip, int port) : base(callback, true)
        {
            IPEndPoint? endPoint = null;
            IPAddress[] addresses = Dns.GetHostAddresses(ip);
            foreach (IPAddress address in addresses)
            {
                endPoint = new IPEndPoint(address, port);
                this.socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                this.socket.NoDelay = true;
                break;
            }

            this.forClient = false;

            Interlocked.Exchange(ref this.closing, 0);
            Interlocked.Exchange(ref this.cleanuped, 0);
            Interlocked.Exchange(ref this.ioRef, 0);

            this.sendPart = new SendPart(this, endPoint);
            this.recvPart = new RecvPart(this, false);
        }

        // Acceptor
        public TcpClientData(IProtocolClientCallback callback, Socket socket, bool forClient) : base(callback, false)
        {
            this.socket = socket;
            this.forClient = forClient;

            Interlocked.Exchange(ref this.closing, 0);
            Interlocked.Exchange(ref this.cleanuped, 0);
            Interlocked.Exchange(ref this.ioRef, 0);

            this.sendPart = new SendPart(this, null);
            this.recvPart = new RecvPart(this, true);
        }

        public override void Connect()
        {
            this.sendPart.Connect();
        }

        public override void Send(byte[] bytes)
        {
            if (this.IsClosing())
            {
                return;
            }

            this.sendPart.Send(bytes);
        }

        // 只调用一次
        public void StartRecv()
        {
            this.recvPart.StartRecv();
        }

        public override void Close(string reason)
        {
            if (Interlocked.Exchange(ref this.closing, 1) != 0)
            {
                return;
            }

            this.closeReason = reason;

            // https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.shutdown?view=net-6.0
            try
            {
                if (this.socket.Connected)
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (SocketException sockEx)
            {
                // https://github.com/mono/mono/issues/7368
                this.callback.LogInfo(sockEx.ToString());
            }
            finally
            {
                this.socket.Close();
            }

            if (Volatile.Read(ref this.ioRef) == 0)
            {
                this.Cleanup();
            }
        }

        public void Cleanup()
        {
            if (Interlocked.Exchange(ref this.cleanuped, 1) != 0)
            {
                return;
            }

            this.sendPart.Cleanup();
            this.recvPart.Cleanup();

            this.callback.OnClose();
        }
    }
}