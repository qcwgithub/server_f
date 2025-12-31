using Data;

namespace Script
{
    public class User_SaveUserInfoToFile : UserHandler<MsgSaveUserInfoToFile, ResSaveUserInfoToFile>
    {
        public User_SaveUserInfoToFile(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_SaveUserInfoToFile;

        public override async Task<ECode> Handle(MsgContext context, MsgSaveUserInfoToFile msg, ResSaveUserInfoToFile res)
        {
            UserInfo? userInfo = null;
            User? user = this.service.sd.GetUser(msg.userId);
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
    }
}