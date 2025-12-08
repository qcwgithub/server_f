using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Command_PerformShowScriptVersion : Handler<CommandService, MsgCommon, ResCommon>
    {
        public override MsgType msgType => MsgType._Command_PerformShowScriptVersion;

        public override async Task<ECode> Handle(ProtocolClientData socket, MsgCommon msg, ResCommon res)
        {
            int serviceId = (int)msg.GetLong("serviceId");

            var msg2 = new MsgGetScriptVersion();
            MyResponse r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._GetScriptVersion, msg2);

            if (r.e == ECode.Success)
            {
                var res = r.CastRes<ResGetScriptVersion>();
                this.service.logger.Info("version: " + res.version);
            }

            return r;
        }
    }
}