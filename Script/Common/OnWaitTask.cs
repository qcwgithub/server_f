using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnWaitTask<S> : Handler<S, MsgWaitTask>
        where S : Service
    {
        public override MsgType msgType => MsgType._WaitTask;
        public sealed override async Task<MyResponse> Handle(ProtocolClientData socket, MsgWaitTask msg)
        {
            await msg.task;
            return ECode.Success;
        }
    }
}