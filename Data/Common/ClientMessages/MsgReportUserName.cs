using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class MsgReportUserName
    {
        [Key(0)]
        public long targetUserId;
        [Key(1)]
        public UserNameReportReason reason;
    }

    [MessagePackObject]
    public class ResReportUserName
    {

    }
}