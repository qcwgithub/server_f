using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnWaitTask<S> : Handler<S, MsgWaitTask, ResWaitTask>
        where S : Service
    {
        public OnWaitTask(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._WaitTask;
        public sealed override async Task<ECode> Handle(MsgContext context, MsgWaitTask msg, ResWaitTask res)
        {
            await msg.task;
            return ECode.Success;
        }
    }
}