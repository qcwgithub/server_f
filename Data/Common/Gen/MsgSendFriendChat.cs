using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSendFriendChat
    {
        [Key(0)]
        public long friendUserId;
        [Key(1)]
        public ChatMessageType chatMessageType;
        [Key(2)]
        public string content;
        [Key(3)]
        public long clientSeq;
        [Key(4)]
        public ChatMessageImageContent? imageContent;
    }
}
