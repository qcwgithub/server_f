using Data;

namespace Script
{
    public partial class RoomService
    {
        public async Task<ECode> SaveFriendChatInfo(FriendChatRoom privateRoom, string reason)
        {
            await this.server.roomLocationRedisW.WriteLocation(privateRoom.roomId, this.serviceId, this.sd.saveIntervalS + 60);

            var msgDb = new MsgSave_FriendChatInfo
            {
                roomId = privateRoom.roomId,
                privateRoomInfoNullable = new FriendChatInfoInfoNullable()
            };
            var infoNullable = msgDb.privateRoomInfoNullable;

            List<string>? buffer = null;
            if (privateRoom.lastFriendChatInfo == null)
            {
                this.logger.Error($"SaveRoom room.lastPrivateRoomInfo == null");
                return ECode.Error;
            }

            FriendChatInfo last = privateRoom.lastFriendChatInfo;
            FriendChatInfo curr = privateRoom.friendChatInfo;

            #region auto

            if (last.roomId != curr.roomId)
            {
                infoNullable.roomId = curr.roomId;
                last.roomId = curr.roomId;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("roomId");
            }
            if (last.createTimeS != curr.createTimeS)
            {
                infoNullable.createTimeS = curr.createTimeS;
                last.createTimeS = curr.createTimeS;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("createTimeS");
            }
            if (last.seq != curr.seq)
            {
                infoNullable.seq = curr.seq;
                last.seq = curr.seq;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("seq");
            }
            if (last.users.IsDifferent_ListClass(curr.users))
            {
                infoNullable.users = curr.users;
                last.users.DeepCopyFrom_ListClass(curr.users);
                if (buffer == null) buffer = new List<string>();
                buffer.Add("users");
            }

            #endregion auto

            // player.lastSceneInfo = curr; // 先假设一定成功吧
            if (last.IsDifferent(curr))
            {
                this.logger.Error("last.IsDifferent(curr)!!!");
            }

            string fieldsStr = "";
            if (buffer != null)
            {
                fieldsStr = string.Join(", ", buffer.ToArray());

                this.logger.InfoFormat("SaveRoom roomId {0}, reason {1}, fields [{2}]", privateRoom.roomId, reason, fieldsStr);
            }

            if (buffer != null)
            {
#if DEBUG
                msgDb.friendChatInfo_debug = FriendChatInfo.Ensure(null);
                msgDb.friendChatInfo_debug.DeepCopyFrom(curr);
#endif
                var r = await this.dbServiceProxy.Save_FriendChatInfo(msgDb);
                if (r.e != ECode.Success)
                {
                    this.logger.ErrorFormat("{0} error: {1}, roomId {2}", MsgType._Save_FriendChatInfo, r.e, privateRoom.roomId);
                    return r.e;
                }
            }

            return ECode.Success;
        }
    }
}