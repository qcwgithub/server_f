using Data;

namespace Script
{
    public class OnGetConnectedInfos<S> : Handler<S, MsgGetConnectedInfos, ResGetConnectedInfos>
        where S : Service
    {
        public OnGetConnectedInfos(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._GetConnectedInfos;
        public override async Task<ECode> Handle(IConnection connection, MsgGetConnectedInfos msg, ResGetConnectedInfos res)
        {
            ServiceData sd = this.service.data;
        
            res.connectedInfos = new List<ServiceTypeAndId>();

            foreach (List<ServiceConnection> list in sd.otherServiceConnections2)
            {
                if (list == null)
                {
                    continue;
                }

                foreach (ServiceConnection soc in list)
                {
                    if (soc == null || soc.serviceTypeAndId == null || !soc.IsConnected())
                    {
                        continue;
                    }

                    res.connectedInfos.Add(soc.serviceTypeAndId.Value);
                }
            }

            return ECode.Success;
        }
    }
}