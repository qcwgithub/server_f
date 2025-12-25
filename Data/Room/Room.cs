namespace Data
{
    public sealed class Room
    {
        public readonly RoomInfo roomInfo;
        public Room(RoomInfo roomInfo)
        {
            this.roomInfo = roomInfo;
        }

        public long roomId
        {
            get
            {
                return this.roomInfo.roomId;
            }
        }

        public bool destroying;
        public ITimer? destroyTimer;

        public ITimer? saveTimer;

        //// 2 ////
        public RoomInfo? lastRoomInfo;
    }
}