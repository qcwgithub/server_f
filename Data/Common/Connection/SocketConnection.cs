using System.Net.Sockets;

namespace Data
{
    public class SocketConnection : IConnection, IProtocolClientCallback
    {
        public readonly ServiceData serviceData;
        public ProtocolClientData socket;
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

        // Connector
        public SocketConnection(ServiceData serviceData, string ip, int port)
        {
            this.serviceData = serviceData;

            this.socket = new TcpClientData(this, ip, port);

            this.isConnector = true;
            this.forClient = false;
        }

        // Acceptor
        public SocketConnection(ServiceData serviceData, Socket socket, bool forClient)
        {
            this.serviceData = serviceData;

            // !
            socket.NoDelay = true;

            this.socket = new TcpClientData(this, socket);

            this.isConnector = false;
            this.forClient = forClient;
        }

        IConnectionCallback callback
        {
            get
            {
                if (this.isConnector)
                {
                    return this.serviceData.connectionCallbackForS;
                }
                return this.forClient ? this.serviceData.connectionCallbackForC : this.serviceData.connectionCallbackForS;
            }
        }

        #region IProtocolClientCallback

        IMessagePacker packer
        {
            get
            {
                return this.serviceData.serverData.messagePacker;
            }
        }
        void IProtocolClientCallback.LogError(string str)
        {
            this.serviceData.logger.Error(str);
        }
        void IProtocolClientCallback.LogError(string str, Exception ex)
        {
            this.serviceData.logger.Error(str, ex);
        }
        void IProtocolClientCallback.LogInfo(string str)
        {
            this.serviceData.logger.Info(str);
        }
        void IProtocolClientCallback.OnConnect(bool success)
        {
            if (success)
            {
                this.callback.OnConnectSuccess(this);
            }
        }
        void IProtocolClientCallback.OnClose()
        {
            this.TimeoutAllWaitings();

            this.callback.OnClose(this);
        }

        int IProtocolClientCallback.OnReceive(byte[] buffer, int offset, int count)
        {
            int used = 0;
            while (this.packer.IsCompeteMessage(buffer, offset, count, out int exactCount))
            {
                UnpackResult r = this.packer.Unpack(buffer, offset, exactCount);
                this.OnMsg(r.seq, r.code, r.msg, r.requireResponse);

                used += r.totalLength;

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
            return used;
        }

        void OnMsg(int seq, int code, ArraySegment<byte> msg, bool requireResponse)
        {
            try
            {
                if (seq > 0)
                {
                    MsgType msgType = (MsgType)code;
                    if (this.forClient && msgType < MsgType.ClientStart)
                    {
                        this.serviceData.logger.Error("receive invalid message from client! " + msgType.ToString());
                        if (requireResponse)
                        {
                            this.socket.Send(this.packer.Pack((int)ECode.Exception, null, -seq, false));
                        }
                        return;
                    }

                    if (!requireResponse)
                    {
                        this.callback.OnMsg(this, seq, msgType, msg, null);
                    }
                    else
                    {
                        this.callback.OnMsg(this, seq, msgType, msg,
                            (ECode e2, ArraySegment<byte> msg2) =>
                            {
                                // 消息处理是异步的，在回复的时候，有可能已经断开了。因此这里要加个判断
                                if (!this.IsClosed())
                                {
                                    this.socket.Send(this.packer.Pack((int)e2, msg2, -seq, false));
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
                        st.callback(eCode, msg);
                    }
                    else
                    {
                        this.serviceData.logger.Error("No response fun for " + (-seq));
                    }
                }
                else
                {
                    this.serviceData.logger.Error("onMsg wrong seq: " + seq);
                }
            }
            catch (Exception ex)
            {
                this.serviceData.logger.Error("ProtocolClientData.OnMsg " + ex);
            }
        }

        #endregion IProtocolClientCallback

        public void Connect()
        {
            this.socket.Connect();
        }

        public bool IsConnecting()
        {
            return this.socket.IsConnecting();
        }

        public bool IsConnected()
        {
            return this.socket.IsConnected();
        }

        public bool IsClosed()
        {
            return this.socket.IsClosed();
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

        public void Send(MsgType msgType, object msg, ReplyCallback? cb, int? pTimeoutS)
        {
            if (!this.IsConnected())
            {
                if (cb != null)
                {
                    cb(ECode.NotConnected, default);
                }
                return;
            }

            var seq = this.serviceData.msgSeq++;
            if (seq <= 0)
            {
                seq = 1;
            }

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

            byte[] msgBytes = MessageTypeConfigData.SerializeMsg(msgType, msg);

            IMessagePacker packer = this.serviceData.serverData.messagePacker;
            var bytes = packer.Pack((int)msgType, msgBytes, seq, cb != null);
            this.socket.Send(bytes);
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