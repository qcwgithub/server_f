using Data;

namespace Script
{
    public class UserConnectionCallbackScript : ConnectionCallbackScript
    {
        public UserConnectionCallbackScript(Server server, Service service) : base(server, service)
        {
        }

        public UserService userService
        {
            get
            {
                return (UserService)this.service;
            }
        }

        public override async void OnMsg(IConnection connection, int seq, MsgType msgType, byte[] msgBytes, ReplyCallback? reply)
        {
            IServiceConnection? serviceConnection = connection as IServiceConnection;
            if (serviceConnection == null)
            {
                this.service.logger.ErrorFormat("OnMsg serviceConnection == null, it is of type '{0}'", connection.GetType().Name);
                return;
            }

            bool received = await Forwarding.S_from_G(this.userService, serviceConnection, msgType, msgBytes, reply);
            if (!received)
            {
                base.OnMsg(connection, seq, msgType, msgBytes, reply);
            }
        }

        public override async void OnLocalMsg(IConnection connection, int seq, MsgType msgType, object msg, LocalReplyCallback? reply)
        {
            IServiceConnection? serviceConnection = connection as IServiceConnection;
            if (serviceConnection == null)
            {
                this.service.logger.ErrorFormat("OnLocalMsg serviceConnection == null, it is of type '{0}'", connection.GetType().Name);
                return;
            }

            bool received = await LocalForwarding.S_from_G(this.userService, serviceConnection, msgType, msg, reply);
            if (!received)
            {
                base.OnLocalMsg(connection, seq, msgType, msg, reply);
            }
        }
    }
}