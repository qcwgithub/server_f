using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgInsert_PrivateRoomInfo
    {
        [Key(0)]
        public PrivateRoomInfo privateRoomInfo;
    }

    [MessagePackObject]
    public class ResInsert_PrivateRoomInfo
    {

    }
}