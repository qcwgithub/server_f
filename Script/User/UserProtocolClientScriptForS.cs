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

        public override async void ReceiveFromNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback reply)
        {
            ServiceConnection? serviceConnection = data.customData as ServiceConnection;
            if (serviceConnection == null)
            {
                this.service.logger.Error("serviceConnection == null");
                return;
            }

            bool received = await Forwarding.TryReceiveClientMessageFromGateway(this.userService, serviceConnection, msgType, msgBytes, reply);
            if (!received)
            {
                base.ReceiveFromNetwork(data, seq, msgType, msgBytes, reply);
            }
        }
    }
}