using Data;

namespace Script
{
    public partial class RoomService
    {
        public async Task<ECode> SaveRoom(Room room, string reason)
        {
            await this.server.roomLocationRedisW.WriteLocation(room.roomId, this.serviceId, this.sd.saveIntervalS + 60);

            var msgDb = new MsgSave_RoomInfo
            {
                roomId = room.roomId,
                roomInfoNullable = new RoomInfoNullable()
            };
            var infoNullable = msgDb.roomInfoNullable;

            List<string>? buffer = null;
            if (room.lastRoomInfo == null)
            {
                this.logger.Error($"SaveRoom room.lastRoomInfo == null");
                return ECode.Error;
            }

            RoomInfo last = room.lastRoomInfo;
            RoomInfo curr = room.roomInfo;

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

            #endregion auto

            // player.lastRoomInfo = curr; // 先假设一定成功吧
            if (last.IsDifferent(curr))
            {
                this.logger.Error("last.IsDifferent(curr)!!!");
            }

            string fieldsStr = "";
            if (buffer != null)
            {
                fieldsStr = string.Join(", ", buffer.ToArray());

                this.logger.InfoFormat("SaveRoom roomId {0}, reason {1}, fields [{2}]", room.roomId, reason, fieldsStr);
            }

            if (buffer != null)
            {
#if DEBUG
                msgDb.roomInfo_debug = RoomInfo.Ensure(null);
                msgDb.roomInfo_debug.DeepCopyFrom(curr);
#endif
                var r = await this.dbServiceProxy.Save_RoomInfo(msgDb);
                if (r.e != ECode.Success)
                {
                    this.logger.ErrorFormat("{0} error: {1}, roomId {2}", MsgType._Save_RoomInfo, r.e, room.roomId);
                    return r.e;
                }
            }

            return ECode.Success;
        }
    }
}