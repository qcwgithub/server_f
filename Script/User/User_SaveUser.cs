using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_SaveUser : UserHandler
    {
        public override MsgType msgType => MsgType._User_SaveUser;
        public override async Task<MyResponse> Handle(ProtocolClientData socket/* null */, object _msg)
        {
            var msg = Utils.CastObject<MsgSaveUser>(_msg);
            long userId = msg.userId;

            var player = this.usData.GetUser(userId);
            if (player == null)
            {
                this.logger.ErrorFormat("{0} place: {1}, userId: {2}, user == null!!", this.msgType, msg.place, userId);
                return ECode.UserNotExist;
            }

            var msgSave = new MsgDatabaseSaveUser();
            msgSave.userId = userId;
            msgSave.profileNullable = new ProfileNullable();
            var profileNullable = msgSave.profileNullable;

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
                msgSave.profile_debug = Profile.Ensure(null);
                msgSave.profile_debug.DeepCopyFrom(curr);
#endif
                MyResponse r = await this.service.connectToDatabaseService.SendAsync(MsgType._Database_SaveUser, msgSave);
                if (r.err != ECode.Success)
                {
                    this.service.logger.ErrorFormat("_Database_SaveUser error: {0}, userId: {1}", r.err, userId);
                    return r;
                }
            }

            return ECode.Success;
        }
    }
}