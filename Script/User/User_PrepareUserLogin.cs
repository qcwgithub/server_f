using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_PrepareUserLogin : UserHandler<MsgPrepareUserLogin, ResPrepareUserLogin>
    {
        public User_PrepareUserLogin(Server server, UserService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._User_PrepareUserLogin;

        public override async Task<ECode> Handle(IConnection connection, MsgPrepareUserLogin msg, ResPrepareUserLogin res)
        {
            long userId = msg.userId;

            this.logger.InfoFormat("{0} userId: {1}", this.msgType, userId);

            if (this.service.data.state != ServiceState.Started)
            {
                if (this.service.data.state != ServiceState.Starting)
                {
                    this.service.logger.ErrorFormat("{0} serverState {1}", this.msgType, this.service.data.state);
                }
                return ECode.Error;
            }

            User? user = this.usData.GetUser(userId);
            if (user != null)
            {
                if (user.destroying)
                {
                    // 其实不是错误，但是想要知道一下有没有触发这种情况
                    this.service.logger.ErrorFormat("{0} userId {1} destroying", this.msgType, user.userId);
                    return ECode.UserDestroying;
                }

                var oldConnection = user.connection;
                if (user.connection != null)
                {
                    User_UserLogin.HandleOldConnection(this.service, user);
                }
            }

            if (user == null)
            {
                ECode e;
                Profile? profile;

                (e, profile) = await this.service.ss.QueryUserProfile(userId);
                if (e != ECode.Success)
                {
                    return e;
                }

                if (profile == null)
                {
                    logger.Info($"user profile {userId} not exist, create a new one!");

                    profile = this.service.ss.NewProfile(userId);
                    e = await this.service.ss.InsertUserProfile(profile);
                    if (e != ECode.Success)
                    {
                        return e;
                    }

                }

                user = new User(profile);
                this.AddPlayerToDict(user);

                // 这里不再加东西了，要加得加到 AddPlayerToDict 里
            }
            else
            {
                // this.gameScripts.CallInit(user);
            }

            user.SetRealPrepareLogin(msg);

            this.InitDestroyTimer_and_SaveTimer(user, this.msgType);
            
            res.playerCount = this.service.sd.userDict.Count;
            return ECode.Success;
        }

        // 正式登录、模拟登录共用
        public void AddPlayerToDict(User user)
        {
            // runtime 初始化
            this.service.sd.userDict.Add(user.userId, user);

            // 有值就不能再赋值了，不然玩家上线下线就错了
            user.lastProfile = Profile.Ensure(null);
            user.lastProfile.DeepCopyFrom(user.profile);

            // qiucw
            // 这句会修改profile，必须放在 lastProfile.DeepCopyFrom 后面
            // this.gameScripts.CallInit(player);
        }

        // 正式登录、模拟登录共用
        public void InitDestroyTimer_and_SaveTimer(User user, MsgType msgType)
        {
            if (!user.destroyTimer.IsAlive())
            {
                this.service.ss.SetDestroyTimer(user, msgType.ToString());
            }

            if (!user.saveTimer.IsAlive())
            {
                this.service.ss.SetSaveTimer(user);
            }
        }
    }
}