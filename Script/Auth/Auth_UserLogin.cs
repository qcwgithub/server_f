using Data;

namespace Script
{
    public class Auth_UserLogin : AuthHandler<MsgUserLogin, ResUserLogin>
    {
        public Auth_UserLogin(Server server, AuthService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Auth_UserLogin;

        public override async Task<ECode> Handle(IConnection connection, MsgUserLogin msg, ResUserLogin res)
        {
            return ECode.Success;
        }
    }
}