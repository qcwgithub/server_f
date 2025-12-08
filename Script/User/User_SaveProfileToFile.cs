
// 运维，GM功能
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_SaveProfileToFile : UserHandler<MsgSaveProfileToFile, ResSaveProfileToFile>
    {
        public override MsgType msgType => MsgType._SaveProfileToFile;

        public override async Task<ECode> Handle(ProtocolClientData socket, MsgSaveProfileToFile msg, ResSaveProfileToFile res)
        {
            Profile? profile = null;
            User? user = this.service.sd.GetUser(msg.userId);
            if (user != null)
            {
                profile = user.profile;
            }

            if (profile == null)
            {
                ECode e;
                // 立刻加载
                (e, profile) = await this.service.ss.QueryUserProfile(msg.userId);
                if (e != ECode.Success)
                {
                    return e;
                }
                if (profile == null)
                {
                    return ECode.UserNotExist;
                }
            }

            string json = JsonUtils.stringify(profile);
            string fileName = "profile" + msg.userId + ".json";
            File.WriteAllText(fileName, json);
            
            res.fileName = fileName;
            return ECode.Success;
        }
    }
}