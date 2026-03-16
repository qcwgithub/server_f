using Data;
using MessagePack;

namespace Script
{
    [AutoRegister]
    public class Room_SendSceneChatTest : Handler<RoomService, MsgRoomSendSceneChatTest, ResRoomSendSceneChatTest>
    {
        public Room_SendSceneChatTest(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SendSceneChatTest;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomSendSceneChatTest msg, ResRoomSendSceneChatTest res)
        {
            string log = $"{this.msgType} roomId {msg.roomId} count {msg.count}";
            this.service.logger.Info(log);

            ServerConfig.MessageConfig messageConfig = this.server.data.serverConfig.sceneMessageConfig;

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

            long now = TimeUtils.GetTime();

            for (int i = 0; i < msg.count; i++)
            {
                // create message
                long seq = ++sceneRoom.roomInfo.messageSeq;
                ChatMessage message = RoomChatScript.CreateChatMessage(
                    seq: seq,
                    roomId: sceneRoom.roomId,
                    senderId: 0,
                    senderName: string.Empty,
                    senderAvatar: string.Empty,
                    type: ChatMessageType.System,
                    content: "Test_" + seq,
                    timestamp: now,
                    replyTo: 0,
                    senderAvatarIndex: 0,
                    clientSeq: 0,
                    status: ChatMessageStatus.Normal,
                    imageContent: null
                );

                // -> redis
                await this.server.sceneMessagesRedis.Add(message);

                if (messageConfig.maxMessagesCount != -1 && message.seq % 100 == 0)
                {
                    await this.server.sceneMessagesRedis.Trim(sceneRoom.roomId, messageConfig.maxMessagesCount);
                }

                // -> memory
                sceneRoom.recentMessages.Enqueue(message);
                while (sceneRoom.recentMessages.Count > messageConfig.recentMessagesCount)
                {
                    sceneRoom.recentMessages.Dequeue();
                }
            }

            this.service.logger.Info($"{log} Done");
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomSendSceneChatTest msg, ECode e, ResRoomSendSceneChatTest res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}