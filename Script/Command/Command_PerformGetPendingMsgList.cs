using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Command_PerformGetPendingMsgList : Handler<CommandService, MsgCommon, ResCommon>
    {
        public Command_PerformGetPendingMsgList(Server server, CommandService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Command_PerformGetPendingMsgList;

        public override async Task<ECode> Handle(MessageContext context, MsgCommon msg, ResCommon res)
        {
            int serviceId = (int)msg.GetLong("serviceId");

            var msg2 = new MsgGetPendingMsgList();
            var r = await this.service.commandConnectToOtherService.Request(serviceId, MsgType._Service_GetPendingMessageList, msg2);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var res2 = r.CastRes<ResGetPendingMsgList>();

            var dict = new Dictionary<MsgType, int>();
            for (int j = 0; j < res2.list.Count; j++)
            {
                int v = res2.list[j];
                if (v != -1)
                {
                    MsgType t = (MsgType)v;
                    if (dict.ContainsKey(t))
                    {
                        dict[t]++;
                    }
                    else
                    {
                        dict[t] = 1;
                    }
                }
            }
            this.service.logger.InfoFormat("serviceId {0} pendingDict {1} list.Count {2}", serviceId, JsonUtils.stringify(dict), res2.list.Count);
            return ECode.Success;
        }
    }
}