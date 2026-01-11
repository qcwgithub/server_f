using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;

namespace Data
{
    public class TcpClientData : ProtocolClientData
    {
        #region variables
        Socket socket;

        bool sending;

        ////        
        bool closed;
        public override bool IsClosed() => this.closed;
        public override EndPoint RemoteEndPoint => this.socket.RemoteEndPoint;

        IPEndPoint ipEndPointForConnector; // when isConnector == true
        SocketAsyncEventArgs _innArgs;
        SocketAsyncEventArgs _outArgs;

        ConcurrentQueue<byte[]> sendQueue = new();
        byte[] recvBuffer = new byte[8192];
        int recvOffset = 0;

        #endregion

        #region constructor

        void _initConnectSocket(string ip, int port)
        {
            IPAddress[] addresses = Dns.GetHostAddresses(ip);
            foreach (IPAddress address in addresses)
            {
                this.ipEndPointForConnector = new IPEndPoint(address, port);
                this.socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                this.socket.NoDelay = true;
                break;
            }
        }

        // Connector
        public TcpClientData(IProtocolClientCallback callback, string ip, int port) : base(callback, true)
        {
            this._initConnectSocket(ip, port);

            this._innArgs = new SocketAsyncEventArgs();
            this._outArgs = new SocketAsyncEventArgs();
            this._innArgs.Completed += this.OnSomethingComplete;
            this._outArgs.Completed += this.OnSomethingComplete;

            this.sending = false;
            this.closed = false;
        }

        // Acceptor
        public TcpClientData(IProtocolClientCallback callback, Socket socket) : base(callback, false)
        {
            this.socket = socket;

            this._innArgs = new SocketAsyncEventArgs();
            this._outArgs = new SocketAsyncEventArgs();
            this._innArgs.Completed += this.OnSomethingComplete;
            this._outArgs.Completed += this.OnSomethingComplete;

            this.sending = false;
            this.closed = false;

            this.PerformRecv();
            this.PerformSend();
        }

        #endregion

        #region OnComplete entry

        void OnSomethingComplete(object? sender, SocketAsyncEventArgs e)
        {
            if (this.closed)
            {
                return;
            }
            try
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Connect:
                        {
                            this.OnConnectComplete(e);
                        }
                        break;
                    case SocketAsyncOperation.Disconnect:
                        {
                            this.OnDisconnectComplete(e);
                        }
                        break;
                    case SocketAsyncOperation.Send:
                        {
                            this.OnSendComplete(e);
                        }
                        break;
                    case SocketAsyncOperation.Receive:
                        {
                            this.OnRecvComplete(e);
                        }
                        break;
                    default:
                        this.callback.LogError("TcpClientData.onSomethingComplete default: " + e.LastOperation);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.callback.LogError("onSomethingComplete " + ex);
            }
        }

        #endregion

        #region connect

        public override void Connect()
        {
            try
            {
                this._outArgs.RemoteEndPoint = this.ipEndPointForConnector;
                bool completed = !this.socket.ConnectAsync(this._outArgs);
                if (completed)
                {
                    this.OnConnectComplete(this._outArgs);
                }
            }
            catch (SocketException ex)
            {
                this.callback.LogError("connect exception" + ex);
            }
        }

        void OnConnectComplete(SocketAsyncEventArgs e)
        {
            e.RemoteEndPoint = null;

            bool success = e.SocketError == SocketError.Success;
            if (success)
            {
                byte[] bytes = this.SendIdentity();
                if (bytes != null)
                {
                    this.sendQueue.Enqueue(bytes);
                }
            }

            this.callback.OnConnect(success);
            if (!success)
            {
                this.Close(CloseReason.OnConnectComplete_false);
            }
            else
            {
                this.PerformRecv();
                this.PerformSend();
            }
        }
        #endregion

        #region send

        void PerformSend()
        {
            if (this.sending || this.sendQueue.Count == 0)
            {
                return;
            }

            this.sending = true;

            if (this.sendQueue.TryDequeue(out byte[]? bytes))
            {
                this.SendAsync(bytes, 0, bytes.Length);
            }
        }

        void SendAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                this._outArgs.SetBuffer(buffer, 0, buffer.Length);
                bool completed = !this.socket.SendAsync(this._outArgs);
                if (completed)
                {
                    this.OnSendComplete(this._outArgs);
                }
            }
            catch (Exception e)
            {
                this.Close("sendAsync Exception " + e);
            }
        }

        public override void Send(byte[] bytes)
        {
            this.sendQueue.Enqueue(bytes);
            this.PerformSend();
        }

        public void OnSendComplete(SocketAsyncEventArgs e)
        {
            this.sending = false;
            if (e.SocketError != SocketError.Success)
            {
                this.Close("onTcpClientComplete_SocketAsyncOperation.Send_SocketError." + e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                this.Close("onTcpClientComplete_SocketAsyncOperation.Send_e.BytesTransferred == 0");
                return;
            }
            this.PerformSend();
        }
        #endregion

        #region recv

        void PerformRecv()
        {
            this.RecvAsync(this.recvBuffer, this.recvOffset, this.recvBuffer.Length - this.recvOffset);
        }

        void RecvAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                this._innArgs.SetBuffer(buffer, offset, count);
                bool completed = !this.socket.ReceiveAsync(this._innArgs);
                if (completed)
                {
                    this.OnRecvComplete(this._innArgs);
                }
            }
            catch (Exception e)
            {
                this.Close("recvAsync Exception " + e);
            }
        }

        void OnRecvComplete(SocketAsyncEventArgs e)
        {
            if (this.closed)
            {
                return;
            }

            if (e.SocketError != SocketError.Success)
            {
                this.Close("onRecvComplete SocketError." + e.SocketError);
                return;
            }

            // https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socketasynceventargs.bytestransferred?view=netcore-3.1
            // If zero is returned from a read operation, the remote end has closed the connection.
            if (e.BytesTransferred == 0)
            {
                this.Close("onRecvComplete e.BytesTransferred == 0");
                return;
            }

            try
            {
                this.recvOffset += e.BytesTransferred;
                int offset = 0;
                int count = this.recvOffset;

                if (this.isAcceptor && !this.identityVerified)
                {
                    var r = this.VerifyIdentity(this.recvBuffer, offset, count, out int identityLength);
                    if (r == VerifyIdentityResult.Success)
                    {
                        offset += identityLength;
                        count -= identityLength;
                    }
                    else if (r == VerifyIdentityResult.Failed)
                    {
                        this.Close("VerifyIdentityFailed");
                    }
                }

                if (!this.isAcceptor || this.identityVerified)
                {
                    int used = this.callback.OnReceive(this.recvBuffer, offset, count);
                    offset += used;
                    count -= used;
                }

                if (!this.closed)
                {
                    if (offset > 0)
                    {
                        Array.Copy(this.recvBuffer, offset, this.recvBuffer, 0, count);
                        this.recvOffset = count;
                    }

                    if (this.recvOffset >= this.recvBuffer.Length)
                    {
                        var newBuffer = new byte[this.recvBuffer.Length * 2];
                        Array.Copy(this.recvBuffer, newBuffer, this.recvOffset);
                        this.recvBuffer = newBuffer;
                    }
                }
            }
            catch (Exception ex)
            {
                this.callback.LogError("OnRecvComplete " + ex);
            }
            finally
            {
                if (!this.closed)
                {
                    // continue recv
                    this.PerformRecv();
                }
            }
        }
        #endregion

        #region close

        public override void Close(string reason)
        {
            if (this.closed)
            {
                // this.logError(this, $"call close on socketId({this.socketId}) with reason({reason}), but this.closed is true!");
                return;
            }
            // this.logInfo(this, $"call close on socketId({this.socketId}) with reason({reason})");
            this.closed = true;
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

            this.sending = false;

            this._innArgs.Completed -= this.OnSomethingComplete;
            this._innArgs.Dispose();
            this._innArgs = null;

            this._outArgs.Completed -= this.OnSomethingComplete;
            this._outArgs.Dispose();
            this._outArgs = null;


            this.sendQueue = null;
            this.recvBuffer = null;

            this.callback.OnClose();
        }

        void OnDisconnectComplete(SocketAsyncEventArgs e)
        {
            this.Close("onDisconnectComplete");
        }

        #endregion
    }
}