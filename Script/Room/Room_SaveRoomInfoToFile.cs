using Data;

namespace Script
{
    [AutoRegister]
    public class Room_SaveRoomInfoToFile : Handler<RoomService, MsgSaveRoomInfoToFile, ResSaveRoomInfoToFile>
    {
        public Room_SaveRoomInfoToFile(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SaveRoomInfoToFile;

        public override async Task<ECode> Handle(MessageContext context, MsgSaveRoomInfoToFile msg, ResSaveRoomInfoToFile res)
        {
            Room? room = await this.service.LockRoom(msg.roomId, context);
            RoomInfo? roomInfo = null;
            if (room != null)
            {
                roomInfo = room.roomInfo;
            }

            if (roomInfo == null)
            {
                ECode e;
                // 立刻加载
                (e, roomInfo) = await this.service.ss.QueryRoomInfo(msg.roomId);
                if (e != ECode.Success)
                {
                    return e;
                }
                if (roomInfo == null)
                {
                    return ECode.RoomNotExist;
                }
            }

            string json = JsonUtils.stringify(roomInfo);
            string fileName = "room_info_" + msg.roomId + ".json";
            File.WriteAllText(fileName, json);

            res.fileName = fileName;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSaveRoomInfoToFile msg, ECode e, ResSaveRoomInfoToFile res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}