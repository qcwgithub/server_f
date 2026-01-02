using Data;

namespace Script
{
    public partial class CommandService
    {
        public async Task<ECode> PerformShutdown(MsgCommon msg)
        {
            int serviceId = (int)msg.GetLong("serviceId");
            int force = (int)msg.GetLong("force");

            var msgShutdown = new MsgShutdown();
            msgShutdown.force = force == 1;
            var r = await this.commandConnectToOtherService.Request(serviceId, MsgType._Service_Shutdown, msgShutdown);
            return r.e;
        }
    }
}