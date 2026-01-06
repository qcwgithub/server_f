using Data;

namespace Script
{
    public class Gateway_ServerKick : Handler<GatewayService, MsgGatewayServerKick, ResGatewayServerKick>
    {
        public Gateway_ServerKick(Server server, GatewayService service) : base(server, service)
        {

        }

        public override MsgType msgType => MsgType._Gateway_ServerKick;

        public override async Task<ECode> Handle(MessageContext context, MsgGatewayServerKick msg, ResGatewayServerKick res)
        {
            this.service.logger.InfoFormat("{0} userId {1}", this.msgType, msg.userId);

            GatewayUser? user = this.service.sd.GetUser(msg.userId);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            {
                var msgU = new MsgUserServerKick();
                msgU.userId = msg.userId;

                var r = await this.service.userServiceProxy.ServerKick(user.userServiceId, msgU);
                if (r.e != ECode.Success)
                {
                    return r.e;
                }
            }

            {
                ECode e = await this.service.DestroyUser(msg.userId, GatewayDestroyUserReason.ServerKick, new MsgKick
                {
                    flags = msg.logoutSdk ? LogoutFlags.LogoutSdk : LogoutFlags.None,
                });
                if (e != ECode.Success)
                {
                    return e;
                }
            }

            return ECode.Success;
        }
    }
}