using Data;

namespace Script
{
    public class UserProtocolClientScriptForS : ProtocolClientScriptForS
    {
        public UserProtocolClientScriptForS(Server server, Service service) : base(server, service)
        {
        }

        public UserService userService
        {
            get
            {
                return (UserService)this.service;
            }
        }

        public override async void ReceiveFromNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            if (data.customData == null)
            {
                this.service.logger.Error("ReceiveFromNetwork data.customData == null");
                return;
            }

            if (data.customData is SocketConnection)
            {
                base.ReceiveFromNetwork(data, seq, msgType, msgBytes, reply);
                return;
            }

            ServiceConnection? serviceConnection = data.customData as ServiceConnection;
            if (serviceConnection == null)
            {
                this.service.logger.ErrorFormat("ReceiveFromNetwork serviceConnection == null, it is of type '{0}'", data.customData.GetType().Name);
                return;
            }

            bool received = await Forwarding.S_from_G(this.userService, serviceConnection, msgType, msgBytes, reply);
            if (!received)
            {
                base.ReceiveFromNetwork(data, seq, msgType, msgBytes, reply);
            }
        }
    }
}