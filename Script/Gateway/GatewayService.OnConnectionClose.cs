using Data;

namespace Script
{
    public partial class GatewayService
    {
        public override async Task<ECode> OnConnectionClose(IConnection connection)
        {
            await base.OnConnectionClose(connection);

            if (connection is GatewayUserConnection gatewayUserConnection)
            {
                GatewayUser? user = gatewayUserConnection.user;
                if (user != null)
                {
                    this.logger.InfoFormat("OnConnectionClose userId {0} closeReason {1}", user.userId, gatewayUserConnection.closeReason);

                    long nowS = TimeUtils.GetTimeS();
                    user.offlineTimeS = nowS;

                    this.ss.SetDestroyTimer(user, GatewayDestroyUserReason.DestroyTimer_Disconnect);

                    var msgU = new MsgUserDisconnectFromGateway();
                    msgU.userId = user.userId;

                    await this.userServiceProxy.UserDisconnectFromGateway(user.userServiceId, msgU);
                }
                return ECode.Success;
            }

            return ECode.Success;
        }
    }
}