using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgReportMessage
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long messageId;
        [Key(2)]
        public MessageReportReason reason;
    }
}
