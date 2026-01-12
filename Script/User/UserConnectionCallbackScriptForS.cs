using Data;

namespace Script
{
    public class UserConnectionCallbackScriptForS : ConnectionCallbackScriptForS
    {
        public UserConnectionCallbackScriptForS(Server server, Service service) : base(server, service)
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
            if (connection is SocketConnection)
            {
                base.OnMsg(connection, seq, msgType, msgBytes, reply);
                return;
            }

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
    }
}