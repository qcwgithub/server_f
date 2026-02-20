using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomManagerCreateRoom
    {
        [Key(0)]
        public RoomType roomType;
        [Key(1)]
        public List<long> participants;
        [Key(2)]
        public string title;
        [Key(3)]
        public string desc;
    }

    [MessagePackObject]
    public class ResRoomManagerCreateRoom
    {
        [Key(0)]
        public SceneInfo sceneInfo;
    }
}