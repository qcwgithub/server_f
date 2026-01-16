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
                long userId = gatewayUserConnection.userId;
                if (userId > 0)
                {
                    GatewayUser? user = this.sd.GetUser(userId);
                    if (user != null)
                    {
                        this.logger.InfoFormat("OnConnectionClose userId {0} closeReason {1}", user.userId, gatewayUserConnection.closeReason);

                        int userServiceId = user.userServiceId;
                        this.DestroyUser(user, GatewayDestroyUserReason.Disconnect, null);
                        user = null;

                        var msgU = new MsgUserDisconnectFromGateway();
                        msgU.userId = userId;

                        await this.userServiceProxy.UserDisconnectFromGateway(userServiceId, msgU);
                    }
                }
                return ECode.Success;
            }

            return ECode.Success;
        }
    }
}