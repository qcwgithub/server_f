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

            (ECode e, ArraySegment<byte> resBytes) = await this.service.dispatcher.Dispatch(connection, msgType, msgBytes);
            if (reply != null)
            {
                reply(e, resBytes);
            }
        }

        public virtual void OnConnectComplete(ProtocolClientData data, bool success)
        {
            if (!success)
            {
                data.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                return;
            }

            if (data.customData == null)
            {
                MyDebug.Assert(false, "data.customData == null");
                return;
            }

            var connection = (IConnection)data.customData;

            var msg = new MsgOnConnectComplete();
            this.service.dispatcher.Dispatch<MsgOnConnectComplete, ResOnConnectComplete>(connection, MsgType._OnConnectComplete, msg).Forget();
        }

        public virtual void OnCloseComplete(ProtocolClientData data)
        {
            if (data.customData == null)
            {
                return;
            }

            var connection = (IConnection)data.customData;

            var msg = new MsgConnectionClose();
            this.service.dispatcher.Dispatch<MsgConnectionClose, ResConnectionClose>(connection, MsgType._OnConnectionClose, msg).Forget();
        }
    }
}