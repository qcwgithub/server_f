using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class MsgReportRoomMessage
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long messageId;
        [Key(2)]
        public RoomMessageReportReason reason;
    }

    [MessagePackObject]
    public class ResReportRoomMessage
    {

    }
}