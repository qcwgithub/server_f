using Data;

namespace Script
{
    [AutoRegister]
    public class RoomManager_CreatePrivateRoom : Handler<RoomManagerService, MsgRoomManagerCreatePrivateRoom, ResRoomManagerCreatePrivateRoom>
    {
        public RoomManager_CreatePrivateRoom(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._RoomManager_CreatePrivateRoom;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomManagerCreatePrivateRoom msg, ResRoomManagerCreatePrivateRoom res)
        {
            this.service.logger.Info($"{this.msgType} participants {JsonUtils.stringify(msg.participants)}");

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

            long roomId = this.service.roomIdSnowflakeScript.NextRoomId();
            PrivateRoomInfo privateRoomInfo = this.service.roomScript.NewPrivateRoomInfo(roomId);

            long nowS = TimeUtils.GetTimeS();
            foreach (long userId in msg.participants)
            {
                var participant = RoomParticipant.Ensure(null);
                participant.userId = userId;
                participant.joinTimeS = nowS;
                privateRoomInfo.participants.Add(participant);
            }

            ECode e = await this.service.roomScript.InsertPrivateSceneInfo(privateRoomInfo);
            if (e != ECode.Success)
            {
                return e;
            }

            res.privateRoomInfo = privateRoomInfo;
            return ECode.Success;
        }
    }
}