using Data;

namespace Script
{
    public class GatewayServiceScript : ServiceScript<GatewayService>
    {
        public GatewayServiceScript(Server server, GatewayService service) : base(server, service)
        {
        }

        public void SetDestroyTimer(GatewayUser user)
        {
            MyDebug.Assert(!user.destroyTimer.IsAlive());

            var SEC = this.service.sd.destroyTimeoutS;
            this.service.logger.InfoFormat("SetDestroyTimer userId {0}", user.userId);

            user.destroyTimer = this.server.timerScript.SetTimer(
                this.service.serviceId,
                SEC, MsgType._Gateway_DestroyUser,
                new MsgGatewayDestroyUser { userId = user.userId, reason = "SetDestroyTimer", msgKick = null});
        }

        public void ClearDestroyTimer(GatewayUser user)
        {
            MyDebug.Assert(user.destroyTimer.IsAlive());
            this.service.logger.InfoFormat("ClearDestroyTimer userId({0})", user.userId);

            server.timerScript.ClearTimer(user.destroyTimer);
            user.destroyTimer = null;
        }
    }
}