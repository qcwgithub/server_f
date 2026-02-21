namespace Data
{
    public sealed class Room
    {
        public readonly RoomType roomType;
        readonly SceneRoomInfo? _sceneRoomInfo;
        readonly PrivateRoomInfo? _privateRoomInfo;
        public readonly long roomId;
        public Room(SceneRoomInfo sceneRoomInfo)
        {
            this.roomType = RoomType.Scene;
            _sceneRoomInfo = sceneRoomInfo;
            this.roomId = sceneRoomInfo.roomId;
            _privateRoomInfo = null;
        }
        public Room(PrivateRoomInfo privateRoomInfo)
        {
            this.roomType = RoomType.Private;
            _sceneRoomInfo = null;
            this.roomId = privateRoomInfo.roomId;
            _privateRoomInfo = privateRoomInfo;
        }

        public SceneRoomInfo sceneRoomInfo
        {
            get
            {
                return _sceneRoomInfo!;
            }
        }

        public PrivateRoomInfo privateRoomInfo
        {
            get
            {
                return _privateRoomInfo!;
            }
        }

        public ITimer? destroyTimer;
        public ITimer? saveTimer;
        public SceneRoomInfo? lastSceneRoomInfo;
        public PrivateRoomInfo? lastPrivateRoomInfo;

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
        public readonly Queue<ChatMessage> recentMessages = new();
    }
}