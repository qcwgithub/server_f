using Data;

namespace Script
{
    // 连接其他服务器成功
    public class User_OnConnectComplete : OnConnectComplete<UserService>
    {
        public User_OnConnectComplete(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgOnConnectComplete msg, ResOnConnectComplete res)
        {
            var e = await base.Handle(context, msg, res);
            if (e != ECode.Success)
            {
                return e;
            }

            return e;
        }
    }
}