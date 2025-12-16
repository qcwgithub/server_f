using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgGatewayDestroyUser
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public string reason;
        [Key(2)]
        public MsgKick msgKick;
    }

    [MessagePackObject]
    public class ResGatewayDestroyUser
    {

    }
}