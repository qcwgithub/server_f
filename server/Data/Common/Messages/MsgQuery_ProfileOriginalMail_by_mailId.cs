using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ProfileOriginalMail_by_mailId
    {
        [Key(0)]
        public longid mailId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ProfileOriginalMail_by_mailId
    {
        [Key(0)]
        public ProfileOriginalMail result;
    }
}
