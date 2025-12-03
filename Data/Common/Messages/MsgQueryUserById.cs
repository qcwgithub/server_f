using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgQueryUserById
    {
        [Key(0)]
        public long userId;
    }

    [MessagePackObject]
    public class ResQueryUserById
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public List<Profile> list;
    }
}