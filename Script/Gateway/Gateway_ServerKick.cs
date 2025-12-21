using Data;

namespace Script
{
    public class Gateway_ServerKick : GatewayHandler<MsgGatewayServerKick, ResGatewayServerKick>
    {
        public Gateway_ServerKick(Server server, GatewayService service) : base(server, service)
        {

        }

        public override MsgType msgType => MsgType._Gateway_ServerKick;

        public override async Task<ECode> Handle(IConnection connection, MsgGatewayServerKick msg, ResGatewayServerKick res)
        {
            this.service.logger.InfoFormat("{0} userId {1}", this.msgType, msg.userId);

            GatewayUser? user = this.service.sd.GetUser(msg.userId);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            if (user.userServiceId > 0)
            {
                var msgU = new MsgUserServerKick();
                msgU.userId = msg.userId;

                var r = await this.service.connectToUserService.Request<MsgUserServerKick, ResUserServerKick>(MsgType._User_ServerKick, msgU);
                if (r.e != ECode.Success)
                {
                    return r.e;
                }
            }

            {
                var msgD = new MsgGatewayDestroyUser();
                msgD.userId = msg.userId;
                msgD.reason = "Gateway_ServerKick";

                msgD.msgKick = new MsgKick
                {
                    flags = msg.logoutSdk ? LogoutFlags.LogoutSdk : LogoutFlags.None,
                };

                var r = await this.service.connectToSelf.Request<MsgGatewayDestroyUser, ResGatewayDestroyUser>(MsgType._Gateway_DestroyUser, msgD);
                if (r.e != ECode.Success)
                {
                    return r.e;
                }
            }

            return ECode.Success;
        }
    }
}