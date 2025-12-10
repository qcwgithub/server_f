using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Command_PerformShowScriptVersion : Handler<CommandService, MsgCommon, ResCommon>
    {
        public Command_PerformShowScriptVersion(Server server, CommandService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Command_PerformShowScriptVersion;

        public override async Task<ECode> Handle(ProtocolClientData socket, MsgCommon msg, ResCommon res)
        {
            int serviceId = (int)msg.GetLong("serviceId");

            var msg2 = new MsgGetScriptVersion();
            var r = await this.service.connectToSameServerType.RequestToService<MsgGetScriptVersion, ResGetScriptVersion>(serviceId, MsgType._GetScriptVersion, msg2);

            if (r.e == ECode.Success)
            {
                this.service.logger.Info("version: " + r.res.version);
            }

            return r.e;
        }
    }
}