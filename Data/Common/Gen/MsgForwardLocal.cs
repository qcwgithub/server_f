using MessagePack;

namespace Data
{
    public class MsgForwardLocal
    {
        public long userId;
        public long[]? userIds;
        public MsgType innerMsgType;
        public object innerMsg;
    }
}