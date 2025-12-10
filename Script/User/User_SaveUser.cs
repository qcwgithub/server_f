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
        public override async Task<ECode> Handle(ProtocolClientData socket, MsgSaveUser msg, ResSaveUser res)
        {
            long userId = msg.userId;

            var player = this.usData.GetUser(userId);
            if (player == null)
            {
                this.logger.ErrorFormat("{0} place: {1}, userId: {2}, user == null!!", this.msgType, msg.place, userId);
                return ECode.UserNotExist;
            }

            var msgDb = new MsgSaveUserProfile
            {
                userId = userId,
                profileNullable = new ProfileNullable()
            };
            var profileNullable = msgDb.profileNullable;

            List<string>? buffer = null;
            Profile last = player.lastProfile;
            Profile curr = player.profile;

            #region auto

            if (last.userId != curr.userId)
            {
                profileNullable.userId = curr.userId;
                last.userId = curr.userId;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("userId");
            }
            if (last.userName != curr.userName)
            {
                profileNullable.userName = curr.userName;
                last.userName = curr.userName;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("userName");
            }
            if (last.createTimeS != curr.createTimeS)
            {
                profileNullable.createTimeS = curr.createTimeS;
                last.createTimeS = curr.createTimeS;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("createTimeS");
            }
            if (last.lastLoginTimeS != curr.lastLoginTimeS)
            {
                profileNullable.lastLoginTimeS = curr.lastLoginTimeS;
                last.lastLoginTimeS = curr.lastLoginTimeS;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("lastLoginTimeS");
            }

            #endregion auto

            // player.lastProfile = curr; // 先假设一定成功吧
            if (last.IsDifferent(curr))
            {
                this.service.logger.Error("last.IsDifferent(curr)!!!");
            }

            string fieldsStr = "";
            if (buffer != null)
            {
                fieldsStr = string.Join(", ", buffer.ToArray());

                // buffer 不为 null 才打印，不然太多了
                this.logger.InfoFormat("{0} place: {1}, playerId: {2}, fields: [{3}]", this.msgType, msg.place, userId, fieldsStr);
            }

            if (buffer != null)
            {
#if DEBUG
                msgDb.profile_debug = Profile.Ensure(null);
                msgDb.profile_debug.DeepCopyFrom(curr);
#endif
                var r = await this.service.connectToDbService.Send<MsgSaveUserProfile, ResSaveUserProfile>(MsgType._Db_SaveUserProfile, msgDb);
                if (r.e != ECode.Success)
                {
                    this.service.logger.ErrorFormat("_Db_SaveUser error: {0}, userId: {1}", r.e, userId);
                    return r.e;
                }
            }

            return ECode.Success;
        }
    }
}