using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSetGmFlag
    {
        [Key(0)]
        public long startUserId;
        [Key(1)]
        public long endUserId;
    }

    [MessagePackObject]
    public class ResSetGmFlag
    {
        [Key(0)]
        public List<long> listUser;
    }
}