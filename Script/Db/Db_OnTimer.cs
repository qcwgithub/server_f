using Data;

namespace Script
{
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
                    break;

                default:
                    e = await base.Handle(context, msg, res);
                    break;
            }
            return e;
        }
    }
}