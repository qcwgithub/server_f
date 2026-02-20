using Data;

namespace Script
{
    public class ChatUtils
    {
        public static ECode CheckChatMessageType(ChatMessageType e)
        {
            return (e >= 0 && e < ChatMessageType.Count) ? ECode.Success : ECode.ChatInvalidType;
        }
    }
}