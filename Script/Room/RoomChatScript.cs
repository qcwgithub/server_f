using Data;

namespace Script
{
    public class RoomChatScript : ServiceScript<RoomService>
    {
        public RoomChatScript(Server server, RoomService service) : base(server, service)
        {
        }

        public ECode CheckRoomSendChat(MsgRoomSendChat msg, ServerConfig.MessageConfig messageConfig)
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
    }
}