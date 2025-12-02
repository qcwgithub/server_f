using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Command_PerformShowScriptVersion : Handler<CommandService>
    {
        public override MsgType msgType => MsgType._Command_PerformShowScriptVersion;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgCommon>(_msg);
            int serviceId = (int)msg.GetLong("serviceId");

            var msg2 = new MsgGetScriptVersion();
            MyResponse r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._GetScriptVersion, msg2);

            if (r.err == ECode.Success)
            {
                var res = r.CastRes<ResGetScriptVersion>();
                this.service.logger.Info("version: " + res.version);
            }

            return r;
        }
    }
}