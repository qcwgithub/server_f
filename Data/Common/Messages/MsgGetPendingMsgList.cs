using MessagePack;
using System.Collections.Generic;
namespace Data
{
    [MessagePackObject]
    public class MsgGetPendingMsgList
    {
        
    }

    [MessagePackObject]
    public class ResGetPendingMsgList
    {
        [Key(0)]
        public List<int> list;
    }
}