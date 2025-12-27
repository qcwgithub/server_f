using Data;

namespace Script
{
    public class Room_SaveRoom : RoomHandler<MsgSaveRoom, ResSaveRoom>
    {
        public Room_SaveRoom(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SaveRoom;
        public override async Task<ECode> Handle(IConnection connection, MsgSaveRoom msg, ResSaveRoom res)
        {
            Room? room = this.sd.GetRoom(msg.roomId);
            if (room == null)
            {
                this.logger.ErrorFormat("{0} roomId {1}, reason {2}, room == null!!", this.msgType, msg.roomId, msg.reason);
                return ECode.RoomNotExist;
            }

            await this.server.roomLocationRedisW.SetOwningServiceId(msg.roomId, this.service.serviceId, this.service.sd.saveIntervalS + 60);

            var msgDb = new MsgSave_RoomInfo
            {
                roomId = msg.roomId,
                roomInfoNullable = new RoomInfoNullable()
            };
            var infoNullable = msgDb.roomInfoNullable;

            List<string>? buffer = null;
            if (room.lastRoomInfo == null)
            {
                this.service.logger.Error($"{this.msgType} room.lastRoomInfo == null");
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
                this.service.logger.Error("last.IsDifferent(curr)!!!");
            }

            string fieldsStr = "";
            if (buffer != null)
            {
                fieldsStr = string.Join(", ", buffer.ToArray());

                // buffer 不为 null 才打印，不然太多了
                this.logger.InfoFormat("{0} roomId {1}, reason {2}, fields [{3}]", this.msgType, msg.roomId, msg.reason, fieldsStr);
            }

            if (buffer != null)
            {
#if DEBUG
                msgDb.roomInfo_debug = RoomInfo.Ensure(null);
                msgDb.roomInfo_debug.DeepCopyFrom(curr);
#endif
                var r = await this.service.connectToDbService.Request<MsgSave_RoomInfo, ResSave_RoomInfo>(MsgType._Save_RoomInfo, msgDb);
                if (r.e != ECode.Success)
                {
                    this.service.logger.ErrorFormat("{0} error: {1}, roomId {2}", MsgType._Save_RoomInfo, r.e, msg.roomId);
                    return r.e;
                }
            }

            return ECode.Success;
        }
    }
}