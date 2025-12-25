using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSave_RoomInfo
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        required public RoomInfoNullable roomInfoNullable;
        [Key(2)]
        public RoomInfo? roomInfo_debug;
    }

    [MessagePackObject]
    public class ResSave_RoomInfo
    {
    }
}