using Data;

namespace Script
{
    public class RoomUtils
    {
        public static ECode CheckRoomType(RoomType e)
        {
            return (e >= 0 && e < RoomType.Count) ? ECode.Success : ECode.InvalidRoomType;
        }

        public static ECode CheckRoomId(long roomId)
        {
            return roomId > 0 ? ECode.Success : ECode.InvalidRoomId;
        }
    }
}