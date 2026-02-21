using Data;

namespace Script
{
    public partial class RoomService
    {
        public async Task<ECode> SavePrivateRoomInfo(PrivateRoom privateRoom, string reason)
        {
            await this.server.roomLocationRedisW.WriteLocation(privateRoom.roomId, this.serviceId, this.sd.saveIntervalS + 60);

            var msgDb = new MsgSave_PrivateRoomInfo
            {
                roomId = privateRoom.roomId,
                privateRoomInfoNullable = new PrivateRoomInfoNullable()
            };
            var infoNullable = msgDb.privateRoomInfoNullable;

            List<string>? buffer = null;
            if (privateRoom.lastPrivateRoomInfo == null)
            {
                this.logger.Error($"SaveRoom room.lastPrivateRoomInfo == null");
                return ECode.Error;
            }

            PrivateRoomInfo last = privateRoom.lastPrivateRoomInfo;
            PrivateRoomInfo curr = privateRoom.privateRoomInfo;

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
            if (last.messageId != curr.messageId)
            {
                infoNullable.messageId = curr.messageId;
                last.messageId = curr.messageId;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("messageId");
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
                msgDb.privateRoomInfo_debug = PrivateRoomInfo.Ensure(null);
                msgDb.privateRoomInfo_debug.DeepCopyFrom(curr);
#endif
                var r = await this.dbServiceProxy.Save_PrivateRoomInfo(msgDb);
                if (r.e != ECode.Success)
                {
                    this.logger.ErrorFormat("{0} error: {1}, roomId {2}", MsgType._Save_PrivateRoomInfo, r.e, privateRoom.roomId);
                    return r.e;
                }
            }

            return ECode.Success;
        }
    }
}