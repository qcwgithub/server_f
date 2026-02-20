namespace Data
{
    public sealed class Room
    {
        public readonly SceneInfo sceneInfo;
        public Room(SceneInfo sceneInfo)
        {
            this.sceneInfo = sceneInfo;
        }

        public long roomId
        {
            get
            {
                return this.sceneInfo.sceneId;
            }
        }

        public ITimer? destroyTimer;
        public ITimer? saveTimer;
        public SceneInfo? lastSceneInfo;

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