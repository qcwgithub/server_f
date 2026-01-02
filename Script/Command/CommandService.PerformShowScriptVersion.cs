using Data;

namespace Script
{
    public partial class CommandService
    {
        public async Task<ECode> PerformShowScriptVersion(MsgCommon msg)
        {
            int serviceId = (int)msg.GetLong("serviceId");

            var msg2 = new MsgGetScriptVersion();
            var r = await this.commandConnectToOtherService.Request(serviceId, MsgType._Service_GetScriptVersion, msg2);

            if (r.e == ECode.Success)
            {
                this.logger.Info("version: " + r.CastRes<ResGetScriptVersion>().version);
            }

            return r.e;
        }
    }
}