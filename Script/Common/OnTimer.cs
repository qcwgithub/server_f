using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnTimer<S> : Handler<S, MsgTimer, ResTimer>
        where S : Service
    {
        public OnTimer(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Timer;

        public override async Task<ECode> Handle(MessageContext context, MsgTimer msg, ResTimer res)
        {
            ECode e = ECode.Success;
            switch (msg.timerType)
            {
                case TimerType.CheckConnections:
                    e = await this.service.CheckConnections();

                    if (this.service.data.state < ServiceState.ShuttingDown)
                    {
                        this.service.data.timer_CheckConnections = this.server.timerScript.SetTimer(this.service.serviceId, this.service.data.state == ServiceState.Started ? 31 : 1, TimerType.CheckConnections, null);
                    }
                    break;

                case TimerType.Shutdown:
                    this.service.Shutdown(false).Forget();
                    break;

                default:
                    throw new Exception("Not handled TimerType." + msg.timerType);
            }

            return e;
        }
    }
}