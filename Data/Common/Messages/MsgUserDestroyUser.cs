using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgUserDestroyUser
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public string reason;
    }

    [MessagePackObject]
    public class ResUserDestroyUser
    {

    }
}