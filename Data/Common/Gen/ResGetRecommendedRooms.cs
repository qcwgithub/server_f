using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResGetRecommendedRooms
    {
        [Key(0)]
        public List<RoomInfo> roomInfos;
    }
}
