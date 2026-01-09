using Data;

namespace Script
{
    public abstract class ProtocolClientScript : ServiceScript<Service>, IConnectionCallback
    {
        public ProtocolClientScript(Server server, Service service) : base(server, service)
        {
        }

        public void OnConnectComplete(IConnection connection, bool success)
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

        public virtual async void OnMsg(IConnection connection, int seq, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
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