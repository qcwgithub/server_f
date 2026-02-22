using Data;

namespace Script
{
    [AutoRegister]
    public class Room_UserEnterScene : Handler<RoomService, MsgRoomUserEnterScene, ResRoomUserEnterScene>
    {
        public Room_UserEnterScene(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_UserEnterScene;
        public override async Task<ECode> Handle(MessageContext context, MsgRoomUserEnterScene msg, ResRoomUserEnterScene res)
        {
            this.service.logger.Info($"{this.msgType} userId {msg.userId} roomId {msg.roomId} gatewayServiceId {msg.gatewayServiceId}");

            var sceneRoom = await this.service.LockRoom<SceneRoom>(msg.roomId, context);
            if (sceneRoom == null)
            {
                ECode e;
                (e, sceneRoom) = await this.service.ss.LoadSceneRoom(msg.roomId);
                if (e != ECode.Success)
                {
                    return e;
                }
                if (sceneRoom == null)
                {
                    return ECode.RoomNotExist;
                }
            }

            SceneRoomUser? user = sceneRoom.GetUser(msg.userId);
            if (user == null)
            {
                user = new SceneRoomUser();
                user.userId = msg.userId;
                sceneRoom.AddUser(user);
            }

            user.gatewayServiceId = msg.gatewayServiceId;

            // +recentMessages
            res.recentMessages = new List<ChatMessage>();
            foreach (ChatMessage message in sceneRoom.recentMessages)
            {
                if (msg.lastSeq < message.seq)
                {
                    res.recentMessages.Add(message);
                }
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomUserEnterScene msg, ECode e, ResRoomUserEnterScene res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}