using Data;

namespace Script
{
    public class Gateway_Broadcast : GatewayHandler<MsgGatewayBroadcast, ResGatewayBroadcast>
    {
        public Gateway_Broadcast(Server server, GatewayService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Gateway_Broadcast;

        public override abstract Task<ECode> Handle(MessageContext context, MsgGatewayBroadcast msg, ResGatewayBroadcast res)
        {
            
        }
    }
}