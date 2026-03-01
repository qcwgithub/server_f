using Data;

namespace Script
{
    public class RoomChatScript : ServiceScript<RoomService>
    {
        public RoomChatScript(Server server, RoomService service) : base(server, service)
        {
        }

        public ECode CheckSendSceneChat(MsgRoomSendSceneChat msg, ServerConfig.MessageConfig messageConfig)
        {
            switch (msg.type)
            {
                case ChatMessageType.Text:
                    {
                        if (msg.content == null)
                        {
                            return ECode.ChatEmpty;
                        }
                        msg.content = msg.content.Trim();

                        if (msg.content.Length < messageConfig.minLength)
                        {
                            return ECode.ChatTooShort;
                        }
                        if (msg.content.Length > messageConfig.maxLength)
                        {
                            return ECode.ChatTooLong;
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
                            return ECode.ChatAllSpace;
                        }
                    }
                    break;

                case ChatMessageType.Image:
                    {
                        ChatMessageImageContent? imageContent = msg.imageContent;
                        if (imageContent == null)
                        {
                            return ECode.ChatMissingImageContent;
                        }

                        if (string.IsNullOrEmpty(imageContent.url) ||
                            imageContent.width <= 0 ||
                            imageContent.height <= 0 ||
                            imageContent.size <= 0)
                        {
                            return ECode.InvalidParam;
                        }
                    }
                    break;

                case ChatMessageType.System: // Not allowed
                default:
                    return ECode.ChatInvalidType;
            }

            return ECode.Success;
        }

        public ECode CheckSendFriendChat(MsgRoomSendFriendChat msg, ServerConfig.MessageConfig messageConfig)
        {
            switch (msg.type)
            {
                case ChatMessageType.Text:
                    {
                        if (msg.content == null)
                        {
                            return ECode.ChatEmpty;
                        }
                        msg.content = msg.content.Trim();

                        if (msg.content.Length < messageConfig.minLength)
                        {
                            return ECode.ChatTooShort;
                        }
                        if (msg.content.Length > messageConfig.maxLength)
                        {
                            return ECode.ChatTooLong;
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
                            return ECode.ChatAllSpace;
                        }
                    }
                    break;

                case ChatMessageType.Image:
                    {
                        ChatMessageImageContent? imageContent = msg.imageContent;
                        if (imageContent == null)
                        {
                            return ECode.ChatMissingImageContent;
                        }

                        if (string.IsNullOrEmpty(imageContent.url) ||
                            imageContent.width <= 0 ||
                            imageContent.height <= 0 ||
                            imageContent.size <= 0)
                        {
                            return ECode.InvalidParam;
                        }
                    }
                    break;

                case ChatMessageType.System: // Not allowed
                default:
                    return ECode.ChatInvalidType;
            }

            return ECode.Success;
        }

        public ECode CheckChatTooFast(Room room, ServerConfig.MessageConfig messageConfig, long userId, long now)
        {
            if (room.lastSendChatStampDict.TryGetValue(userId, out long lastSendChatStamp) &&
                lastSendChatStamp > 0 && now - lastSendChatStamp < messageConfig.minIntervalMs)
            {
                return ECode.ChatTooFast;
            }

            if (room.sendChatTimestampsDict.TryGetValue(userId, out var sendChatTimestamps) &&
                sendChatTimestamps.Count >= messageConfig.periodMaxCount)
            {
                int count = sendChatTimestamps.Count(ts => now - ts < messageConfig.periodMs);
                if (count >= messageConfig.periodMaxCount)
                {
                    return ECode.ChatTooFast;
                }
            }

            return ECode.Success;
        }

        public void WriteChatStamp(Room room, ServerConfig.MessageConfig messageConfig, long userId, long now)
        {
            room.lastSendChatStampDict[userId] = now;

            if (!room.sendChatTimestampsDict.TryGetValue(userId, out var sendChatTimestamps))
            {
                sendChatTimestamps = new();
                room.sendChatTimestampsDict.Add(userId, sendChatTimestamps);
            }

            sendChatTimestamps.RemoveAll(ts => now - ts >= messageConfig.periodMs);
            sendChatTimestamps.Add(now);
        }

        public static ChatMessage CreateChatMessage(
            long seq,
            long roomId,
            long senderId,
            string senderName,
            string senderAvatar,
            ChatMessageType type,
            string content,
            long timestamp,
            long replyTo,
            int senderAvatarIndex,
            long clientSeq,
            ChatMessageStatus status,
            ChatMessageImageContent? imageContent)
        {
            var message = new ChatMessage();
            message.seq = seq;
            message.roomId = roomId;
            message.senderId = senderId;
            message.senderName = senderName;
            message.senderAvatar = senderAvatar;
            message.type = type;
            message.content = content;
            message.timestamp = timestamp;
            message.replyTo = replyTo;
            message.senderAvatarIndex = senderAvatarIndex;
            message.clientSeq = clientSeq;
            message.status = status;
            message.imageContent = imageContent;
            return message;
        }
    }
}