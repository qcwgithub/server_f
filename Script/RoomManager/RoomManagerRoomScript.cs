using Data;

namespace Script
{
    public class RoomManagerRoomScript : ServiceScript<RoomManagerService>
    {
        public RoomManagerRoomScript(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public async Task<ECode> InsertRoomInfo(RoomInfo roomInfo)
        {
            var msgDb = new MsgInsert_RoomInfo();
            msgDb.roomInfo = roomInfo;

            var r = await this.service.dbServiceProxy.Insert_RoomInfo(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InsertRoomInfo({roomInfo.roomId}) r.e {r.e}");
                return r.e;
            }

            return ECode.Success;
        }

        public RoomInfo NewRoomInfo(long roomId, RoomType roomType)
        {
            RoomInfo roomInfo = RoomInfo.Ensure(null);
            roomInfo.roomId = roomId;
            roomInfo.roomType = roomType;

            long nowS = TimeUtils.GetTimeS();
            roomInfo.createTimeS = nowS;
            return roomInfo;
        }

        public ECode CheckCreateRoom(MsgRoomManagerCreateRoom msg)
        {
            switch (msg.roomType)
            {
                case RoomType.Private:
                    {
                        if (msg.participants == null || msg.participants.Count < 2)
                        {
                            return ECode.InvalidParam;
                        }

                        for (int i = 0; i < msg.participants.Count; i++)
                        {
                            long userId = msg.participants[i];
                            for (int j = i + 1; j < msg.participants.Count; j++)
                            {
                                if (userId == msg.participants[j])
                                {
                                    return ECode.Duplicate;
                                }
                            }
                        }
                    }
                    break;
                case RoomType.Public:
                    {
                        if (string.IsNullOrEmpty(msg.title))
                        {
                            return ECode.InvalidParam;
                        }
                        if (string.IsNullOrEmpty(msg.desc))
                        {
                            return ECode.InvalidParam;
                        }
                    }
                    break;
                default:
                    throw new Exception($"Not handled roomType.{msg.roomType}");
            }

            return ECode.Success;
        }
    }
}