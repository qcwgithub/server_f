using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnWaitTask<S> : Handler<S>
        where S : Service
    {
        public override MsgType msgType => MsgType._WaitTask;
        public sealed override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            await Utils.CastObject<Task>(_msg);
            return ECode.Success;
        }
    }
}