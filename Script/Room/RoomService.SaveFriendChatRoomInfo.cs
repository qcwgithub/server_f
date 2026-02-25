using Data;

namespace Script
{
    public partial class RoomService
    {
        public async Task<ECode> SaveFriendChatRoomInfo(FriendChatRoom privateRoom, string reason)
        {
            await this.server.roomLocationRedisW.WriteLocation(privateRoom.roomId, this.serviceId, this.sd.saveIntervalS + 60);

            var msgDb = new MsgSave_FriendChatRoomInfo
            {
                roomId = privateRoom.roomId,
                roomInfoNullable = new FriendChatRoomInfoNullable()
            };
            var infoNullable = msgDb.roomInfoNullable;

            List<string>? buffer = null;
            if (privateRoom.lastFriendChatRoomInfo == null)
            {
                this.logger.Error($"SaveRoom room.lastPrivateRoomInfo == null");
                return ECode.Error;
            }

            FriendChatRoomInfo last = privateRoom.lastFriendChatRoomInfo;
            FriendChatRoomInfo curr = privateRoom.friendChatRoomInfo;

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
            if (last.messageSeq != curr.messageSeq)
            {
                infoNullable.messageSeq = curr.messageSeq;
                last.messageSeq = curr.messageSeq;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("messageSeq");
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
                msgDb.roomInfo_debug = FriendChatRoomInfo.Ensure(null);
                msgDb.roomInfo_debug.DeepCopyFrom(curr);
#endif
                var r = await this.dbServiceProxy.Save_FriendChatRoomInfo(msgDb);
                if (r.e != ECode.Success)
                {
                    this.logger.ErrorFormat("{0} error: {1}, roomId {2}", MsgType._Save_FriendChatRoomInfo, r.e, privateRoom.roomId);
                    return r.e;
                }
            }

            return ECode.Success;
        }
    }
}