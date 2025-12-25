using Data;

namespace Script
{
    public class GatewayServiceScript : ServiceScript<GatewayService>
    {
        public GatewayServiceScript(Server server, GatewayService service) : base(server, service)
        {
        }

        public void SetDestroyTimer(GatewayUser user, GatewayDestroyUserReason reason)
        {
            if (user.destroyTimer.IsAlive())
            {
                return;
            }

            var SEC = this.service.sd.destroyTimeoutS;
            this.service.logger.Info($"SetDestroyTimer userId {user.userId} reason {reason}");

            user.destroyTimer = this.server.timerScript.SetTimer(
                this.service.serviceId,
                SEC, MsgType._Gateway_DestroyUser,
                new MsgGatewayDestroyUser { userId = user.userId, reason = reason, msgKick = null });
        }

        public void ClearDestroyTimer(GatewayUser user, GatewayClearDestroyTimerReason reason)
        {
            if (user.destroyTimer.IsAlive())
            {
                return;
            }

            this.service.logger.Info($"ClearDestroyTimer userId {user.userId} reason {reason}");

            if (user.destroyTimer != null)
            {
                server.timerScript.ClearTimer(user.destroyTimer);
                user.destroyTimer = null;
            }
        }
    }
}