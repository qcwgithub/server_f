using Data;

namespace Script
{
    public class OnGetConnectedInfos<S> : Handler<S, MsgGetConnectedInfos, ResGetConnectedInfos>
        where S : Service
    {
        public OnGetConnectedInfos(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Service_GetConnectedInfos;
        public override async Task<ECode> Handle(MsgContext context, MsgGetConnectedInfos msg, ResGetConnectedInfos res)
        {
            ServiceData sd = this.service.data;
        
            res.connectedInfos = new List<ServiceTypeAndId>();

            foreach (List<ServiceConnection> list in sd.otherServiceConnections2)
            {
                if (list == null)
                {
                    continue;
                }

                foreach (ServiceConnection serviceConnection in list)
                {
                    if (!serviceConnection.IsConnected())
                    {
                        continue;
                    }

                    res.connectedInfos.Add(serviceConnection.tai);
                }
            }

            return ECode.Success;
        }
    }
}