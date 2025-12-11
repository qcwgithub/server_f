using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnGC<S> : Handler<S, MsgGC, ResGC>
        where S : Service
    {
        public OnGC(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._GC;

        public override async Task<ECode> Handle(IConnection connection, MsgGC msg, ResGC res)
        {
            this.service.logger.Info(this.msgType.ToString());

            System.GC.Collect();

            return ECode.Success;
        }
    }
}