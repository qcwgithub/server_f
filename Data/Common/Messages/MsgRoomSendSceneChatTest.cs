using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomSendSceneChatTest
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public int count;
    }

    [MessagePackObject]
    public class ResRoomSendSceneChatTest
    {
        
    }
}