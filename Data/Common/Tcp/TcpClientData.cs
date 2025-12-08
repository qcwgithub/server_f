using System.Net.Sockets;
using System.Net;

namespace Data
{
    public class TcpClientData : ProtocolClientData
    {
        #region variables
        Socket socket;

        ////
        bool connecting;
        public override bool IsConnecting() => this.connecting;

        ////
        bool connected;
        public override bool IsConnected() => this.connected;

        ////        
        bool closed;
        public override bool IsClosed() => this.closed;
        public override EndPoint RemoteEndPoint => this.socket.RemoteEndPoint;

        public IPEndPoint ipEndPointForConnector; // when isConnector == true
        // public CancellationTokenSource _cancellationTaskSource;
        // public CancellationToken _cancellationToken;
        public SocketAsyncEventArgs _innArgs;
        public SocketAsyncEventArgs _outArgs;

        public List<byte[]> sendList = new List<byte[]>();
        public byte[] recvBuffer = new byte[8192];
        public int recvOffset = 0;

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

        public void ConnectorInit(IProtocolClientCallbackProvider callbackProvider, string ip, int port)
        {
            this.callbackProvider = callbackProvider;
            this.isConnector = true;
            this._initConnectSocket(ip, port);

            // this._cancellationTaskSource = new CancellationTokenSource();
            // this._cancellationToken = this._cancellationTaskSource.Token;
            this._innArgs = new SocketAsyncEventArgs();
            this._outArgs = new SocketAsyncEventArgs();
            this._innArgs.Completed += this._onComplete;
            this._outArgs.Completed += this._onComplete;

            this.socketId = this.callback.nextSocketId;
            this.oppositeIsClient = false;

            this.connecting = false;
            this.connected = false;
            this.sending = false;
            this.closed = false;
            this.remoteWillShutdown = false;
        }

        public void AcceptorInit(IProtocolClientCallbackProvider callbackProvider, Socket socket, bool connectedFromClient)
        {
            this.callbackProvider = callbackProvider;
            this.isConnector = false;

            this.socket = socket;
            // this._cancellationTaskSource = new CancellationTokenSource();
            // this._cancellationToken = this._cancellationTaskSource.Token;
            this._innArgs = new SocketAsyncEventArgs();
            this._outArgs = new SocketAsyncEventArgs();
            this._innArgs.Completed += this._onComplete;
            this._outArgs.Completed += this._onComplete;

            this.socketId = this.callback.nextSocketId;
            this.oppositeIsClient = connectedFromClient;

            this.connecting = false;
            this.connected = true;
            this.sending = false;
            this.closed = false;
            this.remoteWillShutdown = false;

            this.PerformRecv();
            this.PerformSend();
        }
        #endregion

        #region OnComplete entry

        // 这个是多线程调用，因此需要放在 Data 这边
        public void _onComplete(object sender, SocketAsyncEventArgs e)
        {
            ET.ThreadSynchronizationContext.Instance.Post(this.OnSomethingComplete, e);
        }

        void OnSomethingComplete(object _e)
        {
            if (this.closed)
            {
                return;
            }
            try
            {
                var e = (SocketAsyncEventArgs)_e;
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
                        this.callback.LogError(this, "TcpClientData.onSomethingComplete default: " + e.LastOperation);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.callback.LogError(this, "onSomethingComplete " + ex);
            }
        }

        #endregion

        #region connect

        public override void Connect()
        {
            try
            {
                this.connecting = true;
                this._outArgs.RemoteEndPoint = this.ipEndPointForConnector;
                bool completed = !this.socket.ConnectAsync(this._outArgs);
                if (completed)
                {
                    this.OnConnectComplete(this._outArgs);
                }
            }
            catch (SocketException ex)
            {
                this.callback.LogError(this, "connect exception" + ex);
            }
        }

        void OnConnectComplete(SocketAsyncEventArgs e)
        {
            this.connecting = false;
            e.RemoteEndPoint = null;
            if (e.SocketError == SocketError.Success)
            {
                this.connected = true;
            }

            bool success = e.SocketError == SocketError.Success;
            if (success)
            {
                byte[] bytes = this.SendIdentity();
                if (bytes != null)
                {
                    this.sendList.Add(bytes);
                }
            }

            this.callback.OnConnectComplete(this, success);

            if (this.closed)
            {
                return;
            }

            if (success)
            {
                this.PerformRecv();
                this.PerformSend();
            }
            else
            {
                this.callback.LogInfo(this, string.Format("TcpClientData.onConnectComplete, e.SocketError = {0} to: {1}",
                    e.SocketError, this.serviceTypeAndId == null ? "null" : this.serviceTypeAndId.Value.ToString()));
            }
        }
        #endregion

        #region send

        void PerformSend()
        {
            if (!this.connected || this.sending || this.sendList.Count == 0)
            {
                return;
            }

            this.sending = true;

            var bytes = this.sendList[0];
            this.sendList.RemoveAt(0);

            this.SendAsync(bytes, 0, bytes.Length);
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
                // throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
                this.Close("sendAsync Exception " + e);
            }
        }

        public override async Task<(ECode, ArraySegment<byte>)> SendBytesAsync(MsgType type, byte[] msg, int? pTimeoutS)
        {
            if (!this.IsConnected())
            {
                return (ECode.Server_NotConnected, default);
            }

            var cs = new TaskCompletionSource<(ECode, ArraySegment<byte>)>();
            this.SendBytes(type, msg, (e, r) =>
            {
                bool success = cs.TrySetResult((e, r));
                if (!success)
                {
                    Console.WriteLine("!cs.TrySetResult " + type);
                }
            }, pTimeoutS);
            var xxx = await cs.Task;
            return xxx;
        }

        async void TimeoutTrigger(int timeoutS, int seq)
        {
            for (int i = 0; i < timeoutS; i++)
            {
                await Task.Delay(1000);
                if (this.closed)
                {
                    return;
                }

                if (!this.waitingResponseDict.ContainsKey(seq))
                {
                    return;
                }
            }

            stWaitingResponse st;
            if (this.waitingResponseDict.TryGetValue(seq, out st))
            {
                this.waitingResponseDict.Remove(seq);
                st.callback(ECode_Timeout, null);
            }
        }

        protected override void SendBytes(MsgType msgType, byte[] msg, Action<ECode, ArraySegment<byte>> cb, int? pTimeoutS)
        {
            if (!this.IsConnected())
            {
                if (cb != null)
                {
                    cb(ECode_NotConnected, null);
                }
                return;
            }

            var seq = this.callback.nextMsgSeq;
            if (cb != null)
            {
                var st = new stWaitingResponse();
                st.callback = cb;
                this.waitingResponseDict.Add(seq, st);

                if (pTimeoutS != null)
                {
                    this.TimeoutTrigger(pTimeoutS.Value, seq);
                }
            }

            // int length = Encoding.UTF8.GetByteCount(message);
            // var bytes = new byte[length + 4];

            // var bytes = Encoding.UTF8.GetBytes(message);
            // this.sendOnePacket(bytes);
            this.SendPacketIgnoreResult((int)msgType, msg, seq, cb != null);
        }

        protected override void SendPacketIgnoreResult(int msgTypeOrECode, byte[] msg, int seq, bool requireResponse)
        {
            var bytes = this.callback.GetMessagePacker().Pack(msgTypeOrECode, msg, seq, requireResponse);
            this.sendList.Add(bytes);
            this.PerformSend();
        }

        protected override void SendRaw(byte[] buffer)
        {
            int seq = this.callback.nextMsgSeq;
            this.callback.GetMessagePacker().ModifySeq(buffer, seq);
            this.sendList.Add(buffer);
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
                this._innArgs.SetBuffer(buffer, offset, buffer.Length - offset);
                bool completed = !this.socket.ReceiveAsync(this._innArgs);
                if (completed)
                {
                    this.OnRecvComplete(this._innArgs);
                }
            }
            catch (Exception e)
            {
                // throw new Exception($"socket set buffer error: {buffer.Length}, {offset}", e);
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
                    int exactCount;
                    while (this.callback.GetMessagePacker().IsCompeteMessage(this.recvBuffer, offset, count, out exactCount))
                    {
                        UnpackResult r = this.callback.GetMessagePacker().Unpack(this.recvBuffer, offset, exactCount);
                        this.OnMsg(r.seq, r.code, r.msg, r.requireResponse);

                        offset += r.totalLength;
                        count -= r.totalLength;

                        if (r.totalLength <= 0)
                        {
                            break;
                        }

                        if (this.closed)
                        {
                            break;
                        }
                    }
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
                this.callback.LogError(this, "OnRecvComplete " + ex);
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
                this.callback.LogInfo(this, reason + "----" + sockEx.ToString());
            }
            finally
            {
                this.socket.Close();
            }
            this.socket = null;

            this.connected = false;
            this.connecting = false;
            this.sending = false;

            this._innArgs.Completed -= this._onComplete;
            this._innArgs.Dispose();
            this._innArgs = null;

            this._outArgs.Completed -= this._onComplete;
            this._outArgs.Dispose();
            this._outArgs = null;


            this.TimeoutAllWaitings();
            this.callback.OnCloseComplete(this);
            this.waitingResponseDict = null;
            this.sendList = null;
            this.recvBuffer = null;
            this.callbackProvider = null;
        }

        void OnDisconnectComplete(SocketAsyncEventArgs e)
        {
            this.connected = false;
            this.Close("onDisconnectComplete");
        }

        #endregion
    }
}