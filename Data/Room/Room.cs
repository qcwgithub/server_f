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

        public ITimer? destroyTimer;
        public ITimer? saveTimer;
        public RoomInfo? lastRoomInfo;

        public Dictionary<long, RoomUser> userDict = new();
        public int userCount
        {
            get
            {
                return this.userDict.Count;
            }
        }
        public RoomUser? GetUser(long userId)
        {
            return this.userDict.TryGetValue(userId, out RoomUser? user) ? user : null;
        }
        public bool RemoveUser(long userId)
        {
            return this.userDict.Remove(userId);
        }
        public void AddUser(RoomUser user)
        {
            this.userDict.Add(user.userId, user);
        }
    }
}