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
        public override async Task<ECode> Handle(MessageContext context, MsgGetConnectedInfos msg, ResGetConnectedInfos res)
        {
            ServiceData sd = this.service.data;
        
            res.connectedInfos = new List<string>();

            foreach (List<IServiceConnection> list in sd.otherServiceConnections2)
            {
                if (list == null)
                {
                    continue;
                }

                foreach (IServiceConnection serviceConnection in list)
                {
                    if (!serviceConnection.IsConnected())
                    {
                        continue;
                    }

                    res.connectedInfos.Add(serviceConnection.identifierString);
                }
            }

            return ECode.Success;
        }
    }
}