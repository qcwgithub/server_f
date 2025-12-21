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

        async Task<stTransferFromGatewayResult> TransferFromGateway(ServiceConnection serviceConnection, MsgType msgType, ArraySegment<byte> msgBytes, Action<ECode, byte[]>? reply)
        {
            var result = new stTransferFromGatewayResult();

            if (!msgType.IsClient() ||
                serviceConnection.serviceTypeAndId.Value.serviceType != ServiceType.Gateway)
            {
                return result;
            }

            result.transferred = true;

            long userId = BinaryMessagePacker.ReadLong(msgBytes, 0);
            var msgBytes2 = new ArraySegment<byte>(msgBytes.Array!, msgBytes.Offset + 8, msgBytes.Count - 8);

            User? user = this.userService.sd.GetUser(userId);

            MyDebug.Assert(user != null);
            MyDebug.Assert(user.connection != null);
            MyDebug.Assert(user.connection.gatewayServiceId == serviceConnection.serviceTypeAndId.Value.serviceId);

            (ECode e, byte[] resBytes) = await this.service.dispatcher.Dispatch(user.connection, msgType, msgBytes2);
            if (reply != null)
            {
                reply(e, resBytes);
            }

            return result;
        }

        public override async void DispatchNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, Action<ECode, byte[]>? reply)
        {
            var serviceConnection = (ServiceConnection)data.customData;

            stTransferFromGatewayResult result = await this.TransferFromGateway(serviceConnection, msgType, msgBytes, reply);
            if (!result.transferred)
            {
                base.DispatchNetwork(data, seq, msgType, msgBytes, reply);
            }
        }
    }
}