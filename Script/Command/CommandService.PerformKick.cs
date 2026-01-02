using Data;

namespace Script
{
    public partial class CommandService
    {
        public async Task<ECode> PerformKick(MsgCommon msg)
        {
            int serviceId = (int)msg.GetLong("serviceId");
            long userId = msg.GetLong("userId");

            this.logger.InfoFormat("PerformKick {0} {1}", serviceId, userId);

            var msgKick = new MsgGatewayServerKick();
            msgKick.userId = userId;
            msgKick.logoutSdk = true;

            var r = await this.commandConnectToOtherService.Request(serviceId, MsgType._Gateway_ServerKick, msgKick);
            return r.e;
        }
    }
}