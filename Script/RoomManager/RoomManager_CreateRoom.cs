using Data;

namespace Script
{
    [AutoRegister]
    public class RoomManager_CreateRoom : Handler<RoomManagerService, MsgRoomManagerCreateRoom, ResRoomManagerCreateRoom>
    {
        public RoomManager_CreateRoom(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._RoomManager_CreateRoom;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomManagerCreateRoom msg, ResRoomManagerCreateRoom res)
        {
            this.service.logger.Info($"{this.msgType} participants {JsonUtils.stringify(msg.participants)}");

            ECode e = this.service.roomScript.CheckCreateRoom(msg);
            if (e != ECode.Success)
            {
                return e;
            }

            long roomId = this.service.roomIdSnowflakeScript.NextRoomId();
            RoomInfo roomInfo = this.service.roomScript.NewRoomInfo(roomId, msg.roomType);

            switch (msg.roomType)
            {
                case RoomType.Private:
                    {
                        long nowS = TimeUtils.GetTimeS();
                        foreach (long userId in msg.participants)
                        {
                            var participant = RoomParticipant.Ensure(null);
                            participant.userId = userId;
                            participant.joinTimeS = nowS;
                            roomInfo.participants.Add(participant);
                        }
                        roomInfo.title = string.Empty;
                        roomInfo.desc = string.Empty;
                    }
                    break;

                case RoomType.Public:
                    {
                        roomInfo.title = msg.title;
                        roomInfo.desc = msg.desc;
                    }
                    break;
                default:
                    throw new Exception($"Not handled roomType.{msg.roomType}");
            }

            e = await this.service.roomScript.InsertRoomInfo(roomInfo);
            if (e != ECode.Success)
            {
                return e;
            }

            res.roomInfo = roomInfo;
            return ECode.Success;
        }
    }
}