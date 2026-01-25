using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class MsgReportUser
    {
        [Key(0)]
        public long targetUserId;
        [Key(1)]
        public UserReportReason reason;
    }

    [MessagePackObject]
    public class ResReportUser
    {

    }
}