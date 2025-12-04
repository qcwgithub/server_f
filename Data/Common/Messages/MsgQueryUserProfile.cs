using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgQueryUserProfile
    {
        [Key(0)]
        public long userId;
    }

    [MessagePackObject]
    public class ResQueryUserProfile
    {
        [Key(0)]
        public Profile? profile;
    }
}