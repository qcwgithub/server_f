using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class MsgSearchRoom
    {
        [Key(0)]
        public string keyword;
    }

    [MessagePackObject]
    public class ResSearchRoom
    {
        [Key(0)]
        public List<RoomInfo> roomInfos;
    }
}