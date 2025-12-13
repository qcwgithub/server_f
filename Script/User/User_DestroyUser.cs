
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_DestroyUser : UserHandler<MsgDestroyUser, ResDestroyUser>
    {
        public User_DestroyUser(Server server, UserService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._User_DestroyUser;

        public override async Task<ECode> Handle(IConnection connection, MsgDestroyUser msg, ResDestroyUser res)
        {
            var sd = this.service.sd;
            long userId = msg.userId;

            this.service.logger.InfoFormat("{0} place: {1}, userId: {2}, preCount: {3}", this.msgType, msg.place, userId, sd.userDict.Count);

            User? user = sd.GetUser(userId);
            if (user == null)
            {
                logger.InfoFormat("{0} user not exist, userId: {1}", this.msgType, userId);
                return ECode.UserNotExist;
            }

            if (msg.msgKick != null && user.IsConnected())
            {
                user.connection.Send<MsgKick>(MsgType.Kick, msg.msgKick);
            }

            if (user.connection != null)
            {
                user.connection.Close("User_DestroyPlayer"); // PMOnDisconnect
            }

            if (user.destroyTimer.IsAlive())
            {
                this.service.ss.ClearDestroyTimer(user, false);
            }

            // clear save timer
            if (user.saveTimer.IsAlive())
            {
                this.service.ss.ClearSaveTimer(user);
            }

            user.destroying = true;

            // 保存一次
            var msgSave = new MsgSaveUser { userId = userId, place = this.msgType.ToString() };
            // this.service.ProxyDispatch(null, MsgType._PSSavePlayer, msgSave, null);
            var r = await this.service.connectToSelf.Request<MsgSaveUser, ResSaveUser>(MsgType._User_SaveUser, msgSave);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            sd.userDict.Remove(userId);
            // this.server.playerPSRedis.DeletePSId(userId);

            return ECode.Success;
        }
    }
}