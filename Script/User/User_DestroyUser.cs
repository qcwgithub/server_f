
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_DestroyUser : UserHandler<MsgUserDestroyUser, ResUserDestroyUser>
    {
        public User_DestroyUser(Server server, UserService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._User_DestroyUser;

        public override async Task<ECode> Handle(MessageContext context, MsgUserDestroyUser msg, ResUserDestroyUser res)
        {
            var sd = this.service.sd;

            this.service.logger.InfoFormat("{0} userId {1}, reason {2}, preCount {3}", this.msgType, msg.userId, msg.reason, sd.userCount);

            User? user = sd.GetUser(msg.userId);
            if (user == null)
            {
                logger.InfoFormat("{0} user not exist, userId: {1}", this.msgType, msg.userId);
                return ECode.UserNotExist;
            }

            this.service.ss.ClearSaveTimer(user);

            user.destroying = true;

            // 保存一次
            var msgSave = new MsgSaveUser();
            msgSave.userId = msg.userId;
            msgSave.reason = "User_DestroyUser";

            var r = await this.service.SaveUser(msgSave);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            sd.RemoveUser(msg.userId);
            this.service.CheckUpdateRuntimeInfo().Forget();
            return ECode.Success;
        }
    }
}