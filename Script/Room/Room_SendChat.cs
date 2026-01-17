using Data;
using MessagePack;

namespace Script
{
    public class Room_SendChat : Handler<RoomService, MsgRoomSendChat, ResRoomSendChat>
    {
        public Room_SendChat(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SendChat;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomSendChat msg, ResRoomSendChat res)
        {
            this.service.logger.Info($"{this.msgType} userId {msg.userId} roomId {msg.roomId} type {msg.type} content {msg.content}");

            if (msg.type < 0 || msg.type >= ChatMessageType.Count)
            {
                return ECode.Chat_InvalidType;
            }

            var roomMessageConfig = this.server.data.serverConfig.roomMessageConfig;

            switch (msg.type)
            {
                case ChatMessageType.Text:
                    {
                        if (msg.content == null)
                        {
                            return ECode.Chat_Empty;
                        }
                        msg.content = msg.content.Trim();

                        if (msg.content.Length < roomMessageConfig.minLength)
                        {
                            return ECode.Chat_TooShort;
                        }
                        if (msg.content.Length > roomMessageConfig.maxLength)
                        {
                            return ECode.Chat_TooLong;
                        }

                        bool allSpace = true;
                        foreach (char c in msg.content)
                        {
                            if (!char.IsWhiteSpace(c))
                            {
                                allSpace = false;
                                break;
                            }
                        }
                        if (allSpace)
                        {
                            return ECode.Chat_AllSpace;
                        }
                    }
                    break;

                case ChatMessageType.Image:
                    return ECode.NotSupported;

                case ChatMessageType.System: // Not allowed
                default:
                    return ECode.Chat_InvalidType;
            }

            Room? room = await this.service.LockRoom(msg.roomId, context);
            if (room == null)
            {
                return ECode.RoomNotExist;
            }

            RoomUser? user = room.GetUser(msg.userId);
            if (user == null)
            {
                return ECode.UserNotInRoom;
            }

            long now = TimeUtils.GetTime();
            if (user.lastSendChatStamp > 0 && now - user.lastSendChatStamp < roomMessageConfig.minIntervalMs)
            {
                return ECode.Chat_TooFast;
            }

            if (user.sendChatTimestamps.Count >= roomMessageConfig.periodMaxCount)
            {
                int count = user.sendChatTimestamps.Count(ts => now - ts < roomMessageConfig.periodMs);
                if (count >= roomMessageConfig.periodMaxCount)
                {
                    return ECode.Chat_TooFast;
                }
            }

            //// ok

            user.lastSendChatStamp = now;
            user.sendChatTimestamps.RemoveAll(ts => now - ts >= roomMessageConfig.periodMs);
            user.sendChatTimestamps.Add(now);

            // -> redis

            var message = new ChatMessage();
            message.messageId = this.service.roomMessageIdSnowflakeScript.NextRoomMessageId();
            message.roomId = room.roomId;
            message.senderId = user.userId;
            message.senderName = string.Empty;
            message.senderAvatar = string.Empty;
            message.type = msg.type;
            message.content = msg.content;
            message.timestamp = TimeUtils.GetTime();
            message.replyTo = null;

            await this.server.roomMessagesRedis.Add(message);

            // -> memory

            this.service.sd.recentMessages.Enqueue(message);
            while (this.service.sd.recentMessages.Count > 100)
            {
                this.service.sd.recentMessages.Dequeue();
            }

            // -> other users

            Dictionary<int, List<RoomUser>> dict = room.userDict
                .GroupBy(pair => pair.Value.gatewayServiceId, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.ToList());

            foreach (var pair in dict)
            {
                int gatewayServiceId = pair.Key;
                List<RoomUser> roomUsers = pair.Value;

                List<long> userIds = roomUsers.Select(x => x.userId).ToList();
                ECode e = this.service.gatewayServiceProxy.BroadcastToClient(gatewayServiceId, userIds, MsgType.A_RoomChat, message);
                if (e == ECode.NotConnected)
                {
                    this.service.logger.Warn($"{this.msgType} gatewayServiceId {gatewayServiceId} is not connected");
                }
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomSendChat msg, ECode e, ResRoomSendChat res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}