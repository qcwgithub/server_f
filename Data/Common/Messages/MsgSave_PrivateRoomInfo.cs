using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSave_PrivateRoomInfo
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        required public PrivateRoomInfoNullable privateRoomInfoNullable;
        [Key(2)]
        public PrivateRoomInfo? privateRoomInfo_debug;
    }

    [MessagePackObject]
    public class ResSave_PrivateRoomInfo
    {
    }
}