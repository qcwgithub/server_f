using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnGC<S> : Handler<S, MsgGC>
        where S : Service
    {
        public override MsgType msgType => MsgType._GC;

        public override Task<MyResponse> Handle(ProtocolClientData socket, MsgGC msg)
        {
            this.service.logger.Info(this.msgType.ToString());

            System.GC.Collect();

            var res = new ResGC();
            return new MyResponse(ECode.Success, res).ToTask();
        }
    }
}