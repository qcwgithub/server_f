using Data;

namespace Script
{
    public abstract class ConnectionCallbackScript : ServiceScript<Service>, IConnectionCallback
    {
        public ConnectionCallbackScript(Server server, Service service) : base(server, service)
        {
        }

        public void OnConnectComplete(IConnection connection)
        {
            var serviceConnection = connection as ServiceConnection;
            if (serviceConnection == null)
            {
                this.service.logger.ErrorFormat("OnConnectComplete data.customData is not ServiceConnection, it is {0}", connection.GetType().Name);
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

        public void OnCloseComplete(IConnection connection)
        {
            this.service.OnConnectionClose(connection).Forget();
        }
    }
}