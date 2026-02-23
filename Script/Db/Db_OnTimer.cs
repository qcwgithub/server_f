using Data;

namespace Script
{
    [AutoRegister(true)]
    public class Db_OnTimer : OnTimer<DbService>
    {
        public Db_OnTimer(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgTimer msg, ResTimer res)
        {
            ECode e;
            switch (msg.timerType)
            {
                case TimerType.Persistence:
                    e = await this.service.Persistence(false);

                    if (this.service.data.state < ServiceState.ShuttingDown)
                    {
                        this.service.sd.timer_persistence_taskQueueHandler_Loop = this.server.timerScript.SetTimer(this.service.serviceId, 1,  TimerType.Persistence, null);
                    }
                    break;

                default:
                    e = await base.Handle(context, msg, res);
                    break;
            }
            return e;
        }
    }
}