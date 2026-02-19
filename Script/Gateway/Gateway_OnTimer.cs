using Data;

namespace Script
{
    [AutoRegister(true)]
    public class Gateway_OnTimer : OnTimer<GatewayService>
    {
        public Gateway_OnTimer(Server server, GatewayService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgTimer msg, ResTimer res)
        {
            switch (msg.timerType)
            {
                default:
                    return await base.Handle(context, msg, res);
            }
        }
    }
}