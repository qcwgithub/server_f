using Data;

namespace Script
{
    public class Gateway_OnConnectionClose : OnConnectionClose<GatewayService>
    {
        public Gateway_OnConnectionClose(Server server, GatewayService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MsgContext context, MsgConnectionClose msg, ResConnectionClose res)
        {
            await base.Handle(context, msg);

            if (context.connection is GatewayUserConnection gatewayUserConnection)
            {
                GatewayUser user = gatewayUserConnection.user;

                this.service.logger.InfoFormat("{0} userId {1} closeReason {2}", this.msgType, user.userId, gatewayUserConnection.closeReason);

                long nowS = TimeUtils.GetTimeS();
                user.offlineTimeS = nowS;

                this.service.ss.SetDestroyTimer(user, GatewayDestroyUserReason.DestroyTimer_Disconnect);

                var msgU = new MsgUserDisconnectFromGateway();
                msgU.userId = user.userId;

                await this.service.connectWithUserService.DisconnectFromGateway(user.userServiceId, msgU);
                return ECode.Success;
            }

            return ECode.Success;
        }
    }
}