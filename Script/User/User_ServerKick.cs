
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_ServerKick : UserHandler<MsgUserServerKick, ResUserServerKick>
    {
        public User_ServerKick(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_ServerKick;

        public override async Task<ECode> Handle(IConnection connection, MsgUserServerKick msg, ResUserServerKick res)
        {
            User? user = this.service.sd.GetUser(msg.userId);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            var msgD = new MsgUserDestroyUser();
            msgD.userId = msg.userId;
            msgD.reason = nameof(User_ServerKick);

            var r = await this.service.connectToSelf.Request<MsgUserDestroyUser, ResUserDestroyUser>(MsgType._User_DestroyUser, msgD);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            return ECode.Success;
        }
    }
}