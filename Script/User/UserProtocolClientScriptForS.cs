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

        public override async void DispatchNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, Action<ECode, byte[]>? reply)
        {
            var serviceConnection = (ServiceConnection)data.customData;

            bool received = await Forwarding.TryReceiveClientMessageFromGateway(this.userService, serviceConnection, msgType, msgBytes, reply);
            if (!received)
            {
                base.DispatchNetwork(data, seq, msgType, msgBytes, reply);
            }
        }
    }
}