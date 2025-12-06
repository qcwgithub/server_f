using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Global_GetServiceConfigs : Handler<GlobalService, MsgGetServiceConfigs>
    {
        public override MsgType msgType => MsgType._Global_GetServiceConfigs;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, MsgGetServiceConfigs msg)
        {
            // this.service.logger.InfoFormat("{0} {1}/{2} from? {3}{4} why? {5}", this.msgType, msg.serverType, msg.serverId, msg.fromServiceType, msg.fromServiceId, msg.why);

            ResGetServiceConfigs res = await this.service.CreateResGetServiceConfigs();

            return new MyResponse(ECode.Success, res);
        }
    }
}