
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
            User? user = this.service.usData.GetUser(userId);

            if (user == null)
            {
                // 立刻加载
                var msgDb = new MsgQueryUserById();
                msgDb.userId = userId;
                var r = await this.service.connectToDatabaseService.SendAsync(MsgType._Database_QueryUser_byId, msgDb);
                if (r.err != ECode.Success)
                {
                    return r;
                }

                var resPlayers = r.CastRes<ResQueryUserById>();
                if (resPlayers.list.Count == 0)
                {
                    return ECode.UserNotExist;
                }

                user = new User();
                user.profile = Profile.Ensure(resPlayers.list[0]);
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