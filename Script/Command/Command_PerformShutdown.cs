using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Command_PerformShutdown : Handler<CommandService, MsgCommon, ResCommon>
    {
        public Command_PerformShutdown(Server server, CommandService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Command_PerformShutdown;

        public override async Task<ECode> Handle(MessageContext context, MsgCommon msg, ResCommon res)
        {
            int serviceId = (int)msg.GetLong("serviceId");
            int force = (int)msg.GetLong("force");

            var msgShutdown = new MsgShutdown();
            msgShutdown.force = force == 1;
            var r = await this.service.commandConnectToOtherService.Request<MsgShutdown, ResShutdown>(serviceId, MsgType._Service_Shutdown, msgShutdown);
            return r.e;
        }
    }
}