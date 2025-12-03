using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgIterate_listOf_ProfileOriginalMail_by_mailId_nd
    {
        [Key(0)]
        public longid start_mailId;
        [Key(1)]
        public longid end_mailId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResIterate_listOf_ProfileOriginalMail_by_mailId_nd
    {
        [Key(0)]
        public List<ProfileOriginalMail> result;
    }
}
