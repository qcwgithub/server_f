using Data;

namespace Script
{
    public abstract class ProtocolClientScript : ServiceScript<Service>, IProtocolClientCallback
    {
        public ProtocolClientScript(Server server, Service service) : base(server, service)
        {
        }

        public IMessagePacker GetMessagePacker()
        {
            return this.server.messagePacker;
        }

        public void LogError(ProtocolClientData data, string str)
        {
            this.service.logger.Error(str);
        }

        public void LogError(ProtocolClientData data, string str, Exception ex)
        {
            this.service.logger.Error(str, ex);
        }

        public void LogInfo(ProtocolClientData data, string str)
        {
            this.service.logger.Info(str);
        }

        public int nextSocketId
        {
            get
            {
                return this.service.data.socketId++;
            }
        }

        public int nextMsgSeq
        {
            get
            {
                return this.service.data.msgSeq++;
            }
        }

        public virtual async void ReceiveFromNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            IConnection? connection = data.customData as IConnection;
            if (connection == null)
            {
                this.service.logger.Error("connection == null");
                return;
            }

            var context = new MessageContext
            {
                connection = connection,
            };

            var msg = MessageTypeConfigData.DeserializeMsg(msgType, msgBytes);
            var r = await this.service.dispatcher.Dispatch(context, msgType, msg);
            if (reply != null)
            {
                ArraySegment<byte> resBytes = MessageTypeConfigData.SerializeRes(msgType, r.res);
                reply(r.e, resBytes);
            }
        }

        public void OnConnectComplete(ProtocolClientData data, bool success)
        {
            if (!success)
            {
                data.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                return;
            }

            if (data.customData == null)
            {
                this.service.logger.Error("OnConnectComplete data.customData == null");
                return;
            }

            var serviceConnection = data.customData as ServiceConnection;
            if (serviceConnection == null)
            {
                this.service.logger.ErrorFormat("OnConnectComplete data.customData is not ServiceConnection, it is {0}", data.customData.GetType().Name);
                return;
            }

            this.service.OnConnectComplete(serviceConnection).Forget();
        }

        public void OnCloseComplete(ProtocolClientData data)
        {
            if (data.customData == null)
            {
                return;
            }

            this.service.OnConnectionClose((IConnection)data.customData).Forget();
        }
    }
}