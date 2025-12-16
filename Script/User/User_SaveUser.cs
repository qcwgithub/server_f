using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_SaveUser : UserHandler<MsgSaveUser, ResSaveUser>
    {
        public User_SaveUser(Server server, UserService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._User_SaveUser;
        public override async Task<ECode> Handle(IConnection connection, MsgSaveUser msg, ResSaveUser res)
        {
            var player = this.sd.GetUser(msg.userId);
            if (player == null)
            {
                this.logger.ErrorFormat("{0} userId {1}, reason {2}, user == null!!", this.msgType, msg.userId, msg.reason);
                return ECode.UserNotExist;
            }

            var msgDb = new MsgSave_UserInfo
            {
                userId = msg.userId,
                userInfoNullable = new UserInfoNullable()
            };
            var infoNullable = msgDb.userInfoNullable;

            List<string>? buffer = null;
            UserInfo last = player.lastUserInfo;
            UserInfo curr = player.userInfo;

            #region auto

            if (last.userId != curr.userId)
            {
                infoNullable.userId = curr.userId;
                last.userId = curr.userId;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("userId");
            }
            if (last.userName != curr.userName)
            {
                infoNullable.userName = curr.userName;
                last.userName = curr.userName;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("userName");
            }
            if (last.createTimeS != curr.createTimeS)
            {
                infoNullable.createTimeS = curr.createTimeS;
                last.createTimeS = curr.createTimeS;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("createTimeS");
            }
            if (last.lastLoginTimeS != curr.lastLoginTimeS)
            {
                infoNullable.lastLoginTimeS = curr.lastLoginTimeS;
                last.lastLoginTimeS = curr.lastLoginTimeS;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("lastLoginTimeS");
            }

            #endregion auto

            // player.lastUserInfo = curr; // 先假设一定成功吧
            if (last.IsDifferent(curr))
            {
                this.service.logger.Error("last.IsDifferent(curr)!!!");
            }

            string fieldsStr = "";
            if (buffer != null)
            {
                fieldsStr = string.Join(", ", buffer.ToArray());

                // buffer 不为 null 才打印，不然太多了
                this.logger.InfoFormat("{0} userId {1}, reason {2}, fields [{3}]", this.msgType, msg.userId, msg.reason, fieldsStr);
            }

            if (buffer != null)
            {
#if DEBUG
                msgDb.userInfo_debug = UserInfo.Ensure(null);
                msgDb.userInfo_debug.DeepCopyFrom(curr);
#endif
                var r = await this.service.connectToDbService.Request<MsgSave_UserInfo, ResSave_UserInfo>(MsgType._Save_UserInfo, msgDb);
                if (r.e != ECode.Success)
                {
                    this.service.logger.ErrorFormat("{0} error: {1}, userId {2}", MsgType._Save_UserInfo, r.e, msg.userId);
                    return r.e;
                }
            }

            return ECode.Success;
        }
    }
}