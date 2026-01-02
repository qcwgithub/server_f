
using Data;

namespace Script
{
    public class User_ServerKick : UserHandler<MsgUserServerKick, ResUserServerKick>
    {
        public User_ServerKick(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_ServerKick;

        public override async Task<ECode> Handle(MessageContext context, MsgUserServerKick msg, ResUserServerKick res)
        {
            User? user = this.service.sd.GetUser(msg.userId);
            if (user == null)
            {
                res.kicked = false;
                return ECode.Success;
            }

            ECode e = await this.service.DestroyUser(msg.userId, UserDestroyUserReason.ServerKick);
            if (e != ECode.Success)
            {
                return e;
            }

            res.kicked = true;
            return ECode.Success;
        }
    }
}