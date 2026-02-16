using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgForwardToUserService
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public MsgType innerMsgType;
        [Key(2)]
        public byte[] innerMsgBytes;
        [Key(3)]
        public string channel; // used by UserManager
        [Key(4)]
        public string channelUserId; // used by UserManager
    }

    [MessagePackObject]
    public class ResForwardToUserService
    {
        [Key(0)]
        public byte[] innerResBytes;
    }
}