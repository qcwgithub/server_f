using Data;

namespace Script
{
    public partial class GatewayService
    {
        public async Task<ECode> DestroyUser(long userId, GatewayDestroyUserReason reason, MsgKick? msgKick)
        {
            this.logger.InfoFormat("DestroyUser userId {0}, reason {1}, preCount {2}", userId, reason, sd.userCount);

            GatewayUser? user = sd.GetUser(userId);
            if (user == null)
            {
                logger.InfoFormat("DestroyUser user not exist, userId: {0}", userId);
                return ECode.Success;
            }

            if (msgKick != null && user.connection != null && user.connection.IsConnected())
            {
                user.connection.Send(MsgType.Kick, msgKick, null, null);
            }

            if (user.connection != null && user.connection.IsConnected())
            {
                user.connection.Close("Gateway_DestroyUser");
            }

            this.ss.ClearDestroyTimer(user, GatewayClearDestroyTimerReason.Destroy);

            user.destroying = true;

            sd.RemoveUser(userId);
            return ECode.Success;
        }
    }
}