using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResSearchRoom
    {
        [Key(0)]
        public List<RoomInfo> roomInfos;
    }
}
