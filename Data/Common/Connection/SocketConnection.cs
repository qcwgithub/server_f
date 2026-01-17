using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Data
{
    public abstract class SocketConnection : IConnection, IProtocolClientCallback
    {
        public struct stWaitingResponse
        {
            public ReplyCallback callback;
        }

        readonly IConnectionCallbackProvider callbackProvider;
        public IProtocolClient socket;
        public readonly bool isConnector;

        public bool isAcceptor
        {
            get
            {
                return !this.isConnector;
            }
        }
        public readonly bool forClient;
        Dictionary<int, stWaitingResponse> waitingResponseDict = new();
        bool closed = false;
        bool connecting = false;
        bool connected = false;
        int handling;

        // Connector
        public SocketConnection(IConnectionCallbackProvider callbackProvider, string ip, int port)
        {
            this.callbackProvider = callbackProvider;

            this.socket = new TcpClientData(this, ip, port);

            this.isConnector = true;
            this.forClient = false;

            Interlocked.Exchange(ref this.handling, 0);
        }

        // Acceptor
        public SocketConnection(IConnectionCallbackProvider callbackProvider, Socket socket, bool forClient, bool startRecv)
        {
            this.callbackProvider = callbackProvider;

            socket.NoDelay = true; // !

            this.socket = new TcpClientData(this, socket, forClient);

            this.isConnector = false;
            this.forClient = forClient;
            this.connected = true; // !

            Interlocked.Exchange(ref this.handling, 0);

            if (startRecv)
            {
                this.StartRecv();
            }
        }

        public void StartRecv()
        {
            ((TcpClientData)this.socket).StartRecv();
        }

        IConnectionCallback callback
        {
            get
            {
                return this.callbackProvider.GetConnectionCallback();
            }
        }

        public enum SocketEventType
        {
            Connect,
            Receive,
            Close,
        }

        public class SocketEvent
        {
            public SocketEventType eventType;
            public object? eventData;

            public SocketEvent(SocketEventType eventType, object? eventData)
            {
                this.eventType = eventType;
                this.eventData = eventData;
            }
        }

        ConcurrentQueue<SocketEvent> eventQueue = new();
        void EnqueueSocketEvent(SocketEvent e)
        {
            this.eventQueue.Enqueue(e);

            if (Interlocked.CompareExchange(ref this.handling, 1, 0) == 0)
            {
                ET.ThreadSynchronizationContext.Instance.Post(this.HandleSocketEvents);
            }
        }

        // NOTE: Called by socket thread
        ServerConfig.SocketSecurityConfig IProtocolClientCallback.socketSecurityConfig
        {
            get
            {
                return this.callback.socketSecurityConfig;
            }
        }

        // NOTE: Called by socket thread
        void IProtocolClientCallback.LogError(string str)
        {
            this.callback.LogError(str);
        }

        // NOTE: Called by socket thread
        void IProtocolClientCallback.LogError(string str, Exception ex)
        {
            this.callback.LogError(str, ex);
        }

        // NOTE: Called by socket thread
        void IProtocolClientCallback.LogInfo(string str)
        {
            this.callback.LogInfo(str);
        }

        // NOTE: Called by socket thread
        void IProtocolClientCallback.OnConnect(bool success)
        {
            this.EnqueueSocketEvent(new SocketEvent(SocketEventType.Connect, success));
        }

        // NOTE: Called by socket thread
        void IProtocolClientCallback.OnClose()
        {
            this.EnqueueSocketEvent(new SocketEvent(SocketEventType.Close, null));
        }

        // NOTE: Called by socket thread
        int IProtocolClientCallback.OnReceive(byte[] buffer, int offset, int count)
        {
            int used = 0;
            while (this.callback.messagePacker.IsCompeteMessage(buffer, offset, count, out int exactCount))
            {
                if (this.forClient)
                {
                    if (exactCount < 0 || exactCount > this.callback.socketSecurityConfig.maxPacketSize)
                    {
                        this.socket.Close($"Invalid packet size {exactCount}");
                        return 0;
                    }
                }

                UnpackResult r = this.callback.messagePacker.Unpack(buffer, offset, exactCount);
                this.EnqueueSocketEvent(new SocketEvent(SocketEventType.Receive, r));

                used += r.totalLength;
                offset += r.totalLength;
                count -= r.totalLength;

                if (r.totalLength <= 0)
                {
                    break;
                }
            }
            return used;
        }

        void HandleSocketEvents()
        {
            while (this.eventQueue.TryDequeue(out SocketEvent? socketEvent))
            {
                if (this.forClient)
                {
                    if (this.eventQueue.Count > this.callback.socketSecurityConfig.maxRecvQueueCount)
                    {
                        this.socket.Close("eventQueue overflow");
                        return;
                    }
                }

                try
                {
                    switch (socketEvent.eventType)
                    {
                        case SocketEventType.Connect:
                            {
                                this.connecting = false;
                                this.connected = (bool)socketEvent.eventData!;
                                this.callback.OnConnect(this, this.connected);
                            }
                            break;

                        case SocketEventType.Receive:
                            {
                                if (socketEvent.eventData is UnpackResult r)
                                {
                                    this.OnMsg(r.seq, r.code, r.msgBytes, r.requireResponse);
                                }
                                else
                                {
                                    throw new Exception("socketEvent.eventData is not UnpackResult");
                                }
                            }
                            break;

                        case SocketEventType.Close:
                            {
                                this.connected = false;
                                this.closed = true;
                                this.TimeoutAllWaitings();
                                this.callback.OnClose(this);
                            }
                            break;

                        default:
                            throw new Exception("Not handled socket event " + socketEvent.eventType);
                    }
                }
                catch (Exception ex)
                {
                    this.callback.LogError("HandleSocketEvents Exception", ex);
                }
            }

            Interlocked.Exchange(ref this.handling, 0);
        }

        public void Connect()
        {
            this.connecting = true;
            this.socket.Connect();
        }

        public bool IsConnecting()
        {
            return this.connecting;
        }

        public bool IsConnected()
        {
            return this.connected;
        }

        public bool IsClosed()
        {
            return this.closed;
        }

        public void Close(string reason)
        {
            this.socket.Close(reason);
        }

        public string? closeReason
        {
            get
            {
                return this.socket.closeReason;
            }
        }

        // Called by Main thread
        public void Send(MsgType msgType, object msg, ReplyCallback? cb)
        {
            if (!this.IsConnected())
            {
                if (cb != null)
                {
                    cb(ECode.NotConnected, default);
                }
                return;
            }

            var seq = this.callback.nextMsgSeq;
            if (cb != null)
            {
                var st = new stWaitingResponse();
                st.callback = cb;
                this.waitingResponseDict.Add(seq, st);
            }

            byte[] msgBytes = MessageTypeConfigData.SerializeMsg(msgType, msg);

            var bytes = this.callback.messagePacker.Pack((int)msgType, msgBytes, seq, cb != null);
            this.socket.Send(bytes);
        }

        // Called by Main thread
        void OnMsg(int seq, int code, byte[] msgBytes, bool requireResponse)
        {
            try
            {
                if (seq > 0)
                {
                    MsgType msgType = (MsgType)code;
                    if (this.forClient)
                    {
                        if (msgType < MsgType.ClientStart || msgType >= MsgType.Count)
                        {
                            this.callback.LogError($"receive invalid message from client! {msgType}");
                            this.socket.Close($"receive invalid message from client! {msgType}");
                            return;
                        }
                    }

                    if (!requireResponse)
                    {
                        this.callback.OnMsg(this, seq, msgType, msgBytes, null);
                    }
                    else
                    {
                        this.callback.OnMsg(this, seq, msgType, msgBytes,
                            (ECode e2, byte[] msg2) =>
                            {
                                // 消息处理是异步的，在回复的时候，有可能已经断开了。因此这里要加个判断
                                if (!this.IsClosed())
                                {
                                    this.socket.Send(this.callback.messagePacker.Pack((int)e2, msg2, -seq, false));
                                }
                            });
                    }
                }
                //// 2 response message
                else if (seq < 0)
                {
                    // this.server.logger.Info("recv response " + eCode + ", " + msg);

                    ECode eCode = (ECode)code;

                    stWaitingResponse st;
                    if (this.waitingResponseDict.TryGetValue(-seq, out st))
                    {
                        this.waitingResponseDict.Remove(-seq);

                        // st.source.Cancel();
                        // st.source.Dispose();

                        // Console.WriteLine("--waiting {0}, -seq = {1}", this.waitingResponses.Count, seq);
                        st.callback(eCode, msgBytes);
                    }
                    else
                    {
                        this.callback.LogError("No response fun for " + (-seq));
                    }
                }
                else
                {
                    this.callback.LogError("onMsg wrong seq: " + seq);
                }
            }
            catch (Exception ex)
            {
                this.callback.LogError("ProtocolClientData.OnMsg " + ex);
            }
        }

        void TimeoutAllWaitings()
        {
            // timeout all waiting responses
            if (this.waitingResponseDict.Count > 0)
            {
                var list = new List<ReplyCallback>();
                foreach (var kv in this.waitingResponseDict)
                {
                    // kv.Value.source.Cancel();
                    // kv.Value.source.Dispose();
                    list.Add(kv.Value.callback);
                }
                this.waitingResponseDict.Clear();
                foreach (var reply in list)
                {
                    reply(ECode.Timeout, null);
                }
            }
        }
    }
}