using Data;

namespace Script
{
    public class Gateway_OnConnectionClose : OnConnectionClose<GatewayService>
    {
        public Gateway_OnConnectionClose(Server server, GatewayService service) : base(server, service)
        {
        }


        public override async Task<ECode> Handle(IConnection connection, MsgConnectionClose msg, ResConnectionClose res)
        {
            await base.Handle(connection, msg);

            // if (msg.isServer)
            // {
            //     return ECode.Success;
            // }

            return ECode.Success;
        }
    }
}