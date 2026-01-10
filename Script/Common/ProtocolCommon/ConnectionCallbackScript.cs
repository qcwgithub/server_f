using Data;

namespace Script
{
    public abstract class ConnectionCallbackScript : ServiceScript<Service>, IConnectionCallback
    {
        public ConnectionCallbackScript(Server server, Service service) : base(server, service)
        {
        }

        public IMessagePacker messagePacker
        {
            get
            {
                return this.server.data.messagePacker;
            }
        }

        public void LogError(string str)
        {
            this.service.logger.Error(str);
        }

        public void LogError(string str, Exception ex)
        {
            this.service.logger.Error(str, ex);
        }

        public void LogInfo(string str)
        {
            this.service.logger.Info(str);
        }

        public int nextMsgSeq
        {
            get
            {
                int seq = this.service.data.msgSeq++;
                if (seq <= 0)
                {
                    seq = 1;
                }
                return seq;
            }
        }

        public void OnConnect(IConnection connection, bool success)
        {
            if (!success)
            {
                return;
            }

            var serviceConnection = connection as ServiceConnection;
            if (serviceConnection == null)
            {
                this.service.logger.ErrorFormat("OnConnectComplete data.customData is not ServiceConnection, it is {0}", connection.GetType().Name);
                return;
            }

            this.service.OnConnectComplete(serviceConnection).Forget();
        }

        public virtual async void OnMsg(IConnection connection, int seq, MsgType msgType, byte[] msgBytes, ReplyCallback? reply)
        {
            var context = new MessageContext
            {
                connection = connection,
            };

            var msg = MessageTypeConfigData.DeserializeMsg(msgType, msgBytes);
            var r = await this.service.dispatcher.Dispatch(context, msgType, msg);
            if (reply != null)
            {
                byte[] resBytes = MessageTypeConfigData.SerializeRes(msgType, r.res);
                reply(r.e, resBytes);
            }
        }

        public void OnClose(IConnection connection)
        {
            this.service.OnConnectionClose(connection).Forget();
        }
    }
}