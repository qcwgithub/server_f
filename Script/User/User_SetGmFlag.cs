
// 运维，GM功能
using Data;

namespace Script
{
    public class User_SetGmFlag : UserHandler<MsgSetGmFlag, ResSetGmFlag>
    {
        public User_SetGmFlag(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._SetGmFlag;

        public override async Task<ECode> Handle(MsgContext context, MsgSetGmFlag msg, ResSetGmFlag res)
        {
            res.listUser = new List<long>();
            for (long i = msg.startUserId; i <= msg.endUserId; i++)
            {
                User? user = this.service.sd.GetUser(i);
                if (user != null)
                {
                    user.isGm = true;
                    res.listUser.Add(i);
                }
            }
            
            return ECode.Success;
        }
    }
}