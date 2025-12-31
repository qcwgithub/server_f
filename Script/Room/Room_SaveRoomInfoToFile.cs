using Data;

namespace Script
{
    public class Room_SaveRoomInfoToFile : RoomHandler<MsgSaveRoomInfoToFile, ResSaveRoomInfoToFile>
    {
        public Room_SaveRoomInfoToFile(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SaveRoomInfoToFile;

        public override async Task<ECode> Handle(MsgContext context, MsgSaveRoomInfoToFile msg, ResSaveRoomInfoToFile res)
        {
            RoomInfo? roomInfo = null;
            Room? room = this.service.sd.GetRoom(msg.roomId);
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
    }
}