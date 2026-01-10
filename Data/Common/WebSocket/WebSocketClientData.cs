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
        public WebSocketClientData(IProtocolClientCallback callback, string url, int connectTimeoutMs) : base(callback, true)
        {
            this._initConnectSocket(url);

            this.url = url;
            this.connectTimeoutMs = connectTimeoutMs;

            ClientWebSocket clientWebSocket = new ClientWebSocket();
            this.webSocket = clientWebSocket;
        }

        // Acceptor
        public WebSocketClientData(IProtocolClientCallback callback, WebSocket webSocket, IPEndPoint remoteEndPoint) : base(callback, false)
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
                            this.callback?.LogInfo($"ConnectAsync (ignore) SocketErrorCode {se.SocketErrorCode}");
                            break;

                        default:
                            this.callback?.LogError($"ConnectAsync", se);
                            break;
                    }
                }
                // System.Net.WebSockets.WebSocketException (0x80004005): Unable to connect to the remote server
                else if (wsex.HResult == unchecked((int)0x80004005))
                {
                    this.callback?.LogInfo($"ConnectAsync (ignore) {wsex}");
                }
                else
                {
                    this.callback?.LogError($"ConnectAsync", wsex);
                }
            }
            catch (Exception ex)
            {
                this.callback?.LogError($"ConnectAsync", ex);
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
                this.callback.OnConnect(success);
                if (!success)
                {
                    this.Close(CloseReason.OnConnectComplete_false);
                }
                else
                {
                    this.StartRecv();
                }
            }
        }

        #endregion

        #region send

        public override void Send(byte[] bytes)
        {
            this.SendPacket(bytes, CancellationToken.None);
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
                    this.callback?.LogInfo($"SendAsync (ignore) {wsex.Message}");
                }
                else
                {
                    this.callback?.LogError($"SendAsync", wsex);
                }

                this.Close("SendAsync WebSocketException");
            }
            catch (Exception ex)
            {
                this.callback?.LogError($"SendAsync", ex);
                this.Close("SendAsync Exception");
            }
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
                        this.callback?.LogError($"Call 'ReceiveAsync' when this.webSocket.State = {this.webSocket.State}");
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
                        this.callback?.LogInfo($"ReceiveAsync (ignore) {wsex.Message}");
                    }
                    else
                    {
                        this.callback?.LogError($"ReceiveAsync", wsex);
                    }

                    this.Close("ReceiveAsync WebSocketException");
                    return;
                }
                catch (Exception ex)
                {
                    this.callback?.LogError($"ReceiveAsync", ex);
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
                            _ = this.callback.OnReceive(this.recvBuffer, 0, this.recvOffset);
                        }
                        else
                        {
                            this.callback?.LogError($"Receieved unsupported WebSocketMessageType.{result.MessageType}, what now? continue receive.");
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
                    this.callback?.LogError($"ReceiveAsync.2", ex);
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
                                this.callback?.LogError($"CloseAsync", ex);
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
                            this.callback?.LogInfo($"CloseAsync (ignore) SocketErrorCode {se.SocketErrorCode}");
                            break;

                        default:
                            this.callback?.LogError($"CloseAsync", se);
                            break;
                    }
                }
                else
                {
                    this.callback?.LogError($"CloseAsync", wsex);
                }
            }
            catch (Exception ex)
            {
                this.callback?.LogError($"CloseAsync", ex);
            }
            finally
            {
                this.webSocket.Dispose();
                this.webSocket = null;
                this.remoteEndPoint = null;
            }

            this.callback.OnClose();
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