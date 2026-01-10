using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace Data
{
    public class WebSocketClientData : ProtocolClientData
    {
        #region variables
        public WebSocket? webSocket;

        ////
        // bool connecting;
        public override bool IsConnecting() => this.webSocket != null && this.webSocket.State == WebSocketState.Connecting;

        ////
        // bool connected;
        public override bool IsConnected() => this.webSocket != null && this.webSocket.State == WebSocketState.Open;

        ////
        bool closed;
        IPEndPoint remoteEndPoint;
        public override bool IsClosed()
        {
            if (this.closed)
            {
                return true;
            }

            WebSocketState state = this.webSocket.State;
            // return state == WebSocketState.CloseSent || state == WebSocketState.CloseReceived || state == WebSocketState.Closed;
            if (state == WebSocketState.Closed)
            {
                this.Close("IsClosed");
                return true;
            }
            return false;
        }

#if !UNITY_2017_1_OR_NEWER
        public override EndPoint RemoteEndPoint => this.remoteEndPoint;
#endif
        string url;
        int connectTimeoutMs;
        public bool receiving;
        public byte[] recvBuffer;
        public int recvOffset;

        #endregion

        #region constructor

        void _initConnectSocket(string url)
        {

        }

        // Connector
        public WebSocketClientData(IProtocolClientCallback callback, int socketId, string url, int connectTimeoutMs) : base(callback, socketId, true, false)
        {
            this._initConnectSocket(url);

            this.url = url;
            this.connectTimeoutMs = connectTimeoutMs;

            ClientWebSocket clientWebSocket = new ClientWebSocket();
            this.webSocket = clientWebSocket;
        }

        // Acceptor
        public WebSocketClientData(IProtocolClientCallback callback, int socketId, WebSocket webSocket, bool forClient, IPEndPoint remoteEndPoint) : base(callback, socketId, false, forClient)
        {
            this.webSocket = webSocket;
            this.remoteEndPoint = remoteEndPoint;

            this.StartRecv();
        }

        #endregion

        #region connect

        public override async void Connect()
        {
            bool success = false;
            ClientWebSocket clientWebSocket = (ClientWebSocket)this.webSocket;
            var timeout = new CancellationTokenSource(this.connectTimeoutMs);
            try
            {
                await clientWebSocket.ConnectAsync(new Uri(this.url), timeout.Token);

                // this.callback.LogInfo(this, string.Format("after ConnectAsync, state = {0}", clientWebSocket.State));
                success = true;
            }
            catch (TaskCanceledException ex)
            {
                this.callback?.LogInfo(string.Format("connect task canceled" + ex));
            }
            catch (WebSocketException wsex)
            {
                if (wsex.InnerException is SocketException se)
                {
                    switch (se.SocketErrorCode)
                    {
                        case SocketError.HostNotFound:
                        case SocketError.TimedOut:
                        case SocketError.ConnectionRefused:
                            this.callback?.LogInfo($"[{this.socketId}] ConnectAsync (ignore) SocketErrorCode {se.SocketErrorCode}");
                            break;

                        default:
                            this.callback?.LogError($"[{this.socketId}] ConnectAsync", se);
                            break;
                    }
                }
                // System.Net.WebSockets.WebSocketException (0x80004005): Unable to connect to the remote server
                else if (wsex.HResult == unchecked((int)0x80004005))
                {
                    this.callback?.LogInfo($"[{this.socketId}] ConnectAsync (ignore) {wsex}");
                }
                else
                {
                    this.callback?.LogError($"[{this.socketId}] ConnectAsync", wsex);
                }
            }
            catch (Exception ex)
            {
                this.callback?.LogError($"[{this.socketId}] ConnectAsync", ex);
            }
            finally
            {
                timeout.Dispose();
            }

            if (clientWebSocket.State == WebSocketState.Closed)
            {
                this.Close("connect failed");
            }
            else
            {
                this.callback.OnConnectComplete(success);

                if (success)
                {
                    this.StartRecv();
                }
            }
        }

        #endregion

        #region send

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
                st.callback(ECode.Timeout, null);
            }
        }

        public override void SendBytes(MsgType msgType, ArraySegment<byte> msg, int seq, ReplyCallback? cb, int? pTimeoutS)
        {
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

            this.SendPacketIgnoreResult((int)msgType, msg, seq, cb != null);
        }

        async void SendPacket(byte[] bytes, CancellationToken cancellationToken)
        {
            try
            {
                await this.webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, cancellationToken);
            }
            catch (TaskCanceledException)
            {

            }
            catch (WebSocketException wsex)
            {
                bool isBenign = wsex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely;

                if (wsex.InnerException is SocketException se)
                {
                    switch (se.SocketErrorCode)
                    {
                        case SocketError.ConnectionReset:
                        case SocketError.Shutdown:
                            isBenign = true;
                            break;
                    }
                }

                if (isBenign)
                {
                    this.callback?.LogInfo($"[{this.socketId}] SendAsync (ignore) {wsex.Message}");
                }
                else
                {
                    this.callback?.LogError($"[{this.socketId}] SendAsync", wsex);
                }

                this.Close("SendAsync WebSocketException");
            }
            catch (Exception ex)
            {
                this.callback?.LogError($"[{this.socketId}] SendAsync", ex);
                this.Close("SendAsync Exception");
            }
        }

        protected override void SendPacketIgnoreResult(int msgTypeOrECode, ArraySegment<byte> msg, int seq, bool requireResponse)
        {
            var bytes = this.callback.GetMessagePacker().Pack(msgTypeOrECode, msg, seq, requireResponse);
            this.SendPacket(bytes, CancellationToken.None);
        }

        #endregion

        #region recv

        async void StartRecv()
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.net.websockets.clientwebsocket.receiveasync?view=netcore-3.1
            // Exactly one send and one receive is supported on each ClientWebSocket object in parallel.
            if (this.receiving)
            {
                return;
            }
            this.receiving = true;

            while (true)
            {
                if (this.recvBuffer == null)
                {
                    this.recvBuffer = new byte[256];
                    this.recvOffset = 0;
                }

                WebSocketReceiveResult result;
                try
                {
                    if (this.webSocket.State != WebSocketState.Open)
                    {
                        this.callback?.LogError($"[{this.socketId}] Call 'ReceiveAsync' when this.webSocket.State = {this.webSocket.State}");
                    }

                    if (this.webSocket.State == WebSocketState.Closed || this.webSocket.State == WebSocketState.Aborted)
                    {
                        this.Close("webSocket exception closed");
                        return;
                    }

                    int leftSpace = this.recvBuffer.Length - this.recvOffset;
                    result = await this.webSocket.ReceiveAsync(new ArraySegment<byte>(this.recvBuffer, this.recvOffset, leftSpace), CancellationToken.None);
                }
                catch (WebSocketException wsex)
                {
                    bool isBenign = wsex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely;

                    if (wsex.InnerException is SocketException se)
                    {
                        switch (se.SocketErrorCode)
                        {
                            case SocketError.ConnectionReset:
                            case SocketError.Shutdown:
                                isBenign = true;
                                break;
                        }
                    }

                    if (isBenign)
                    {
                        this.callback?.LogInfo($"[{this.socketId}] ReceiveAsync (ignore) {wsex.Message}");
                    }
                    else
                    {
                        this.callback?.LogError($"[{this.socketId}] ReceiveAsync", wsex);
                    }

                    this.Close("ReceiveAsync WebSocketException");
                    return;
                }
                catch (Exception ex)
                {
                    this.callback?.LogError($"[{this.socketId}] ReceiveAsync", ex);
                    this.Close("ReceiveAsync Exception");
                    return;
                }

                try
                {
                    if (result.CloseStatus != null)
                    {
                        this.Close("recv CloseStatus " + result.CloseStatus.ToString(), result.CloseStatus.Value);
                        return;
                    }

                    // this.callback.LogInfo(this, string.Format("[{0}]recv count({1}) closeStatus({2}) type({3}) endOfMessage({4})", this.uid, result.Count, result.CloseStatus, result.MessageType, result.EndOfMessage));

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        this.Close("result.MessageType is Close", WebSocketCloseStatus.NormalClosure);
                        return;
                    }

                    // update offset
                    this.recvOffset += result.Count;

                    if (result.EndOfMessage)
                    {
                        if (result.MessageType == WebSocketMessageType.Binary)
                        {
                            UnpackResult r = this.callback.GetMessagePacker().Unpack(this.recvBuffer, 0, this.recvOffset);
                            if (!r.success)
                            {
                                this.Close("!r.success, ");
                                break;
                            }

                            this.OnMsg(r.seq, r.code, r.msg, r.requireResponse);
                        }
                        else
                        {
                            this.callback?.LogError($"[{this.socketId}]receieved unsupported WebSocketMessageType.{result.MessageType}, what now? continue receive.");
                        }

                        //
                        this.recvOffset = 0;
                    }
                    else if (this.recvOffset >= this.recvBuffer.Length)
                    {
                        // no enough space
                        var newBuffer = new byte[this.recvBuffer.Length * 2];
                        Array.Copy(this.recvBuffer, newBuffer, this.recvOffset);
                        this.recvBuffer = newBuffer;
                    }
                }
                catch (Exception ex)
                {
                    this.callback?.LogError($"[{this.socketId}] ReceiveAsync.2", ex);
                    this.Close("ReceiveAsync.2 Exception");
                }

                if (this.closed)
                {
                    break;
                }

                // Continue Receieve
            }

            this.receiving = false;
        }

        #endregion


        #region close

        // https://github.com/dotnet/runtime/issues/81762
        // What's the differences between ClientWebSocket.CloseAsync and ClientWebSocket.CloseOutputAsync?
        // 源码
        // https://github.com/dotnet/runtime/blob/32ea33964de6e6188aa62afcbc4b1f17f1dbf1ff/src/libraries/System.Net.WebSockets/src/System/Net/WebSockets/ManagedWebSocket.cs#L325-L362
        async void Close(string reason, WebSocketCloseStatus closeStatus)
        {
            if (this.closed)
            {
                return;
            }

            this.closed = true;
            this.closeReason = reason;

            try
            {
                WebSocketState state = this.webSocket.State;
                switch (state)
                {
                    case WebSocketState.Open:
                    case WebSocketState.CloseReceived:
                        {
                            try
                            {
                                await this.webSocket.CloseAsync(closeStatus, reason, CancellationToken.None);
                            }
                            catch (Exception ex)
                            {
                                this.callback?.LogError($"[{this.socketId}] CloseAsync", ex);
                            }
                        }
                        break;

                    case WebSocketState.CloseSent:
                    case WebSocketState.Closed:
                    case WebSocketState.Connecting:
                    default:
                        break;
                }
            }
            catch (WebSocketException wsex)
            {
                // 如果是连接已经被对方断开了，通常可以忽略
                if (wsex.InnerException is SocketException se)
                {
                    switch (se.SocketErrorCode)
                    {
                        // 一些常见的可忽略 Socket 错误码（比如连接已关闭）
                        case SocketError.ConnectionReset:
                        case SocketError.NotConnected:
                            this.callback?.LogInfo($"[{this.socketId}] CloseAsync (ignore) SocketErrorCode {se.SocketErrorCode}");
                            break;

                        default:
                            this.callback?.LogError($"[{this.socketId}] CloseAsync", se);
                            break;
                    }
                }
                else
                {
                    this.callback?.LogError($"[{this.socketId}] CloseAsync", wsex);
                }
            }
            catch (Exception ex)
            {
                this.callback?.LogError($"[{this.socketId}] CloseAsync", ex);
            }
            finally
            {
                this.webSocket.Dispose();
                this.webSocket = null;
                this.remoteEndPoint = null;
            }

            this.TimeoutAllWaitings();
            this.callback.OnCloseComplete(this);
            this.recvBuffer = null;
            this.recvOffset = 0;
        }

        public override void Close(string reason)
        {
            this.Close(reason, WebSocketCloseStatus.NormalClosure);
        }

        #endregion
    }
}