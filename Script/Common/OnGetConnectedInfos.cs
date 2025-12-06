using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnGetConnectedInfos<S> : Handler<S, MsgGetConnectedInfos>
        where S : Service
    {
        public override MsgType msgType => MsgType._GetConnectedInfos;
        public override Task<MyResponse> Handle(ProtocolClientData socket, MsgGetConnectedInfos msg)
        {
            ServiceData sd = this.service.data;

            var res = new ResGetConnectedInfos();
            res.connectedInfos = new List<ServiceTypeAndId>();

            foreach (List<ProtocolClientData> list in sd.otherServiceSockets2)
            {
                if (list == null)
                {
                    continue;
                }

                foreach (ProtocolClientData soc in list)
                {
                    if (soc == null || soc.serviceTypeAndId == null || !soc.IsConnected())
                    {
                        continue;
                    }

                    res.connectedInfos.Add(soc.serviceTypeAndId.Value);
                }
            }

            return new MyResponse(ECode.Success, res).ToTask();
        }
    }
}