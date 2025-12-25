using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgInsert_RoomInfo
    {
        [Key(0)]
        public RoomInfo roomInfo;
    }

    [MessagePackObject]
    public class ResInsert_RoomInfo
    {

    }
}