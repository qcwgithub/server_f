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
            SceneInfo sceneInfo = this.service.roomScript.NewSceneInfo(roomId);

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
                            sceneInfo.participants.Add(participant);
                        }
                        sceneInfo.title = string.Empty;
                        sceneInfo.desc = string.Empty;
                    }
                    break;

                case RoomType.Public:
                    {
                        sceneInfo.title = msg.title;
                        sceneInfo.desc = msg.desc;
                    }
                    break;
                default:
                    throw new Exception($"Not handled roomType.{msg.roomType}");
            }

            e = await this.service.roomScript.InsertSceneInfo(sceneInfo);
            if (e != ECode.Success)
            {
                return e;
            }

            res.sceneInfo = sceneInfo;
            return ECode.Success;
        }
    }
}