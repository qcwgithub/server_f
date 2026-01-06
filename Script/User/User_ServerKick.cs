
using Data;

namespace Script
{
    public class User_ServerKick : Handler<UserService, MsgUserServerKick, ResUserServerKick>
    {
        public User_ServerKick(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_ServerKick;

        public override async Task<ECode> Handle(MessageContext context, MsgUserServerKick msg, ResUserServerKick res)
        {
            User? user = await this.service.LockUser(msg.userId, context);
            if (user == null)
            {
                res.kicked = false;
                return ECode.Success;
            }

            ECode e = await this.service.DestroyUser(user, UserDestroyUserReason.ServerKick);
            if (e != ECode.Success)
            {
                return e;
            }

            res.kicked = true;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgUserServerKick msg, ECode e, ResUserServerKick res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}