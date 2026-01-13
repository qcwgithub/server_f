using Data;

namespace Script
{
    public class OnGetPendingMsgList<S> : Handler<S, MsgGetPendingMsgList, ResGetPendingMsgList>
        where S : Service
    {
        public OnGetPendingMsgList(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._GetPendingMessageList;

        public override async Task<ECode> Handle(MessageContext context, MsgGetPendingMsgList msg, ResGetPendingMsgList res)
        {
            res.list = new List<int>();
            res.list.AddRange(this.service.data.busyList);
            return ECode.Success;
        }
    }
}