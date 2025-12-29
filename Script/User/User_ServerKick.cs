
using Data;

namespace Script
{
    public class User_ServerKick : User_ServerHandler<MsgUserServerKick, ResUserServerKick>
    {
        public User_ServerKick(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_ServerKick;

        public override async Task<ECode> Handle(MsgContext context, MsgUserServerKick msg, ResUserServerKick res)
        {
            User? user = this.service.sd.GetUser(msg.userId);
            if (user == null)
            {
                res.kicked = false;
                return ECode.Success;
            }

            var msgD = new MsgUserDestroyUser();
            msgD.userId = msg.userId;
            msgD.reason = UserDestroyUserReason.ServerKick;

            var r = await this.service.Request<MsgUserDestroyUser, ResUserDestroyUser>(MsgType._User_DestroyUser, msgD);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            res.kicked = true;
            return ECode.Success;
        }
    }
}