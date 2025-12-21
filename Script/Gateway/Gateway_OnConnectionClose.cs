using Data;

namespace Script
{
    public class Gateway_OnConnectionClose : OnConnectionClose<GatewayService>
    {
        public Gateway_OnConnectionClose(Server server, GatewayService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(IConnection connection, MsgConnectionClose msg, ResConnectionClose res)
        {
            await base.Handle(connection, msg);

            if (connection is GatewayUserConnection userConnection)
            {
                GatewayUser user = userConnection.user;

                this.service.logger.InfoFormat("{0} userId {1} closeReason {2}", this.msgType, user.userId, userConnection.closeReason);

                long nowS = TimeUtils.GetTimeS();
                user.offlineTimeS = nowS;

                this.service.ss.SetDestroyTimer(user, GatewayDestroyUserReason.DestroyTimer_Disconnect);

                var msgU = new MsgUserDisconnectFromGateway();
                msgU.userId = user.userId;

                var rU = await this.service.connectToUserService.Request<MsgUserDisconnectFromGateway, ResUserDisconnectFromGateway>(
                    user.userServiceId, MsgType._User_UserDisconnectFromGateway, msgU);
                return ECode.Success;
            }

            return ECode.Success;
        }
    }
}