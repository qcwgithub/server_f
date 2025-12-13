using Data;

namespace Script
{
    public class Gateway_UserLogin : GatewayHandler<MsgUserLogin, ResUserLogin>
    {
        public Gateway_UserLogin(Server server, GatewayService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType.UserLogin;

        public override async Task<ECode> Handle(IConnection connection, MsgUserLogin msg, ResUserLogin res)
        {
            var gatewayUserConnection = (GatewayUserConnection)connection;
            return ECode.Success;
        }
    }
}