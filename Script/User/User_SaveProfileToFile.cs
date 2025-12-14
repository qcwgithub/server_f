
// 运维，GM功能
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_SaveProfileToFile : UserHandler<MsgSaveProfileToFile, ResSaveProfileToFile>
    {
        public User_SaveProfileToFile(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._SaveProfileToFile;

        public override async Task<ECode> Handle(IConnection connection, MsgSaveProfileToFile msg, ResSaveProfileToFile res)
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
            string fileName = "profile" + msg.userId + ".json";
            File.WriteAllText(fileName, json);
            
            res.fileName = fileName;
            return ECode.Success;
        }
    }
}