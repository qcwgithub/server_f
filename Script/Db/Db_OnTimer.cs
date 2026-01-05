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
            switch (msg.timerType)
            {
                case TimerType.PersistenceTaskQueueHandler_Loop:
                    break;

                default:
                    return await base.Handle(context, msg, res);
            }
        }
    }
}