using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;


namespace Script
{
    public class Command_PerformSetPlayerGmFlag : Handler<CommandService, MsgCommon, ResCommon>
    {
        public Command_PerformSetPlayerGmFlag(Server server, CommandService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Command_PerformSetPlayerGmFlag;

        public override async Task<ECode> Handle(IConnection connection, MsgCommon msg, ResCommon res)
        {
            long startId = msg.GetLong("startId");
            long endId = msg.GetLong("endId");
            int serviceId = (int)msg.GetLong("serviceId");

            var msgSet = new MsgSetGmFlag();
            msgSet.startUserId = startId;
            msgSet.endUserId = endId;

            var r = await this.service.commandConnectToOtherService.Request<MsgSetGmFlag, ResSetGmFlag>(serviceId, MsgType._SetGmFlag, msgSet);
            if (r.res.listUser != null)
            {
                foreach (var item in r.res.listUser)
                {
                    this.service.logger.InfoFormat("{0} success set GM!", item);
                }
            }
            return r.e;
        }
    }
}