using Data;

namespace Script
{
    [AutoRegister]
    public class Gateway_Action : Handler<GatewayService, MsgGatewayServiceAction, ResGatewayServiceAction>
    {
        public Gateway_Action(Server server, GatewayService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Gateway_ServerAction;

        public override async Task<ECode> Handle(MessageContext context, MsgGatewayServiceAction msg, ResGatewayServiceAction res)
        {
            this.service.logger.Info(this.msgType);

            return ECode.Success;
        }
    }
}