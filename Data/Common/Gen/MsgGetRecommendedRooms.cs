using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgGetRecommendedRooms
    {

    }

    [MessagePackObject]
    public class ResGetRecommendedRooms
    {
        [Key(0)]
        public List<RoomInfo> roomInfos;
    }
}