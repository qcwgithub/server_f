using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;

namespace Data
{
    public partial class TcpClientData : ProtocolClientData
    {
        Socket socket;

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
        public void IncreaseIORef()
        {
            Interlocked.Increment(ref this.ioRef);
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

            Interlocked.Exchange(ref this.closing, 0);
            Interlocked.Exchange(ref this.cleanuped, 0);
            Interlocked.Exchange(ref this.ioRef, 0);

            this.sendPart = new SendPart(this, endPoint);
            this.recvPart = new RecvPart(this, false);
        }

        // Acceptor
        public TcpClientData(IProtocolClientCallback callback, Socket socket) : base(callback, false)
        {
            this.socket = socket;
            Interlocked.Exchange(ref this.closing, 0);
            Interlocked.Exchange(ref this.cleanuped, 0);
            Interlocked.Exchange(ref this.ioRef, 0);

            this.sendPart = new SendPart(this, null);
            this.recvPart = new RecvPart(this, true);

            this.recvPart.StartRecv();
        }

        public override void Connect()
        {
            this.sendPart.Connect();
        }

        public override void Send(byte[] bytes)
        {
            this.sendPart.Send(bytes);
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
                this.callback.LogInfo(reason + "----" + sockEx.ToString());
            }
            finally
            {
                this.socket.Close();
            }
            this.socket = null;

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