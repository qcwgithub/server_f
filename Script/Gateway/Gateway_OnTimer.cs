using Data;

namespace Script
{
    public class Gateway_OnTimer : OnTimer<GatewayService>
    {
        public Gateway_OnTimer(Server server, GatewayService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgTimer msg, ResTimer res)
        {
            switch (msg.timerType)
            {
                case TimerType.DestroyGatewayUser:
                    {
                        var destroy = msg.data as TimerGatewayDestroyUser;
                        if (destroy == null)
                        {
                            throw new Exception("TimerGatewayDestroyUser data is null");
                        }
                        return await this.service.DestroyUser(destroy.userId, destroy.reason, null);
                    }

                default:
                    return await base.Handle(context, msg, res);
            }
        }
    }
}