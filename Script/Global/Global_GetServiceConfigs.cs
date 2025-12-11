using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Global_GetServiceConfigs : Handler<GlobalService, MsgGetServiceConfigs, ResGetServiceConfigs>
    {
        public Global_GetServiceConfigs(Server server, GlobalService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Global_GetServiceConfigs;

        public override async Task<ECode> Handle(IConnection connection, MsgGetServiceConfigs msg, ResGetServiceConfigs res)
        {
            // this.service.logger.InfoFormat("{0} {1}/{2} from? {3}{4} why? {5}", this.msgType, msg.serverType, msg.serverId, msg.fromServiceType, msg.fromServiceId, msg.why);

            this.service.FillResGetServiceConfigs(res);
            return ECode.Success;
        }
    }
}