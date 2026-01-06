using Data;

namespace Script
{
    public class User_SaveUserInfoToFile : Handler<UserService, MsgSaveUserInfoToFile, ResSaveUserInfoToFile>
    {
        public User_SaveUserInfoToFile(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_SaveUserInfoToFile;

        public override async Task<ECode> Handle(MessageContext context, MsgSaveUserInfoToFile msg, ResSaveUserInfoToFile res)
        {
            User? user = await this.service.LockUser(msg.userId, context);
            UserInfo? userInfo = null;
            if (user != null)
            {
                userInfo = user.userInfo;
            }

            if (userInfo == null)
            {
                ECode e;
                // 立刻加载
                (e, userInfo) = await this.service.ss.QueryUserInfo(msg.userId);
                if (e != ECode.Success)
                {
                    return e;
                }
                if (userInfo == null)
                {
                    return ECode.UserNotExist;
                }
            }

            string json = JsonUtils.stringify(userInfo);
            string fileName = "user_info_" + msg.userId + ".json";
            File.WriteAllText(fileName, json);

            res.fileName = fileName;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSaveUserInfoToFile msg, ECode e, ResSaveUserInfoToFile res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}