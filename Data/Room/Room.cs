namespace Data
{
    public sealed class Room
    {
        public readonly RoomType roomType;
        readonly SceneInfo? _sceneInfo;
        readonly PrivateRoomInfo? _privateRoomInfo;
        public readonly long roomId;
        public Room(SceneInfo sceneInfo)
        {
            this.roomType = RoomType.Public;
            _sceneInfo = sceneInfo;
            this.roomId = sceneInfo.roomId;
            _privateRoomInfo = null;
        }
        public Room(PrivateRoomInfo privateRoomInfo)
        {
            this.roomType = RoomType.Private;
            _sceneInfo = null;
            this.roomId = privateRoomInfo.roomId;
            _privateRoomInfo = privateRoomInfo;
        }

        public SceneInfo sceneInfo
        {
            get
            {
                return _sceneInfo!;
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
        public SceneInfo? lastSceneInfo;
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