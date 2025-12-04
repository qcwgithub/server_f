
// 运维，GM功能
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_SaveProfileToFile : UserHandler
    {
        public override MsgType msgType => MsgType._SaveProfileToFile;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSaveProfileToFile>(_msg);
            long userId = msg.userId;
            User? user = this.service.sd.GetUser(userId);

            if (user == null)
            {
                // 立刻加载
                (ECode e, Profile? profile) = await this.service.ss.QueryUserProfile(userId);
                if (e != ECode.Success)
                {
                    return e;
                }
                if (profile == null)
                {
                    return ECode.UserNotExist;
                }

                user = new User();
                user.profile = Profile.Ensure(profile);
            }

            string json = JsonUtils.stringify(user.profile);
            string fileName = "profile" + msg.userId + ".json";
            File.WriteAllText(fileName, json);

            var res = new ResSaveProfileToFile();
            res.fileName = fileName;
            return new MyResponse(ECode.Success, res);
        }
    }
}