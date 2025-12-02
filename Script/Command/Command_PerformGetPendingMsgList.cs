using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Command_PerformGetPendingMsgList : Handler<CommandService>
    {
        public override MsgType msgType => MsgType._Command_PerformGetPendingMsgList;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgCommon>(_msg);
            int serviceId = (int)msg.GetLong("serviceId");

            var msg2 = new MsgGetPendingMsgList();
            MyResponse r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._GetPendingMessageList, msg2);
            if (r.err != ECode.Success)
            {
                return r;
            }

            var res = r.CastRes<ResGetPendingMsgList>();
            var dict = new Dictionary<MsgType, int>();
            for (int j = 0; j < res.list.Count; j++)
            {
                int v = res.list[j];
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
            this.service.logger.InfoFormat("serviceId {0} pendingDict {1} list.Count {2}", serviceId, JsonUtils.stringify(dict), res.list.Count);
            return r;
        }
    }
}