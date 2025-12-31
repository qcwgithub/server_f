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
            var r = await this.service.commandConnectToOtherService.Request<MsgGetPendingMsgList, ResGetPendingMsgList>(serviceId, MsgType._Service_GetPendingMessageList, msg2);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var dict = new Dictionary<MsgType, int>();
            for (int j = 0; j < r.res.list.Count; j++)
            {
                int v = r.res.list[j];
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
            this.service.logger.InfoFormat("serviceId {0} pendingDict {1} list.Count {2}", serviceId, JsonUtils.stringify(dict), r.res.list.Count);
            return ECode.Success;
        }
    }
}