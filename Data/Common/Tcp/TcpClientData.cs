using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;

namespace Data
{
    public partial class TcpClientData : ProtocolClientData
    {
        Socket socket;

        ////
        int closed;
        public override bool IsClosed()
        {
            return Volatile.Read(ref this.closed) == 1;
        }
        public override EndPoint RemoteEndPoint => this.socket.RemoteEndPoint;

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

            Interlocked.Exchange(ref this.closed, 0);

            this.sendPart = new SendPart(this, endPoint);
            this.recvPart = new RecvPart(this, false);
        }

        // Acceptor
        public TcpClientData(IProtocolClientCallback callback, Socket socket) : base(callback, false)
        {
            this.socket = socket;
            Interlocked.Exchange(ref this.closed, 0);

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
            if (Interlocked.CompareExchange(ref this.closed, 1, 0) != 0)
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

            this.sendPart.Destroy();
            this.recvPart.Destroy();

            this.callback.OnClose();
        }
    }
}