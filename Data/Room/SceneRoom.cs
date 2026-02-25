namespace Data
{
    public class SceneRoom : Room
    {
        public readonly SceneRoomInfo roomInfo;
        public SceneRoom(SceneRoomInfo roomInfo)
        {
            this.roomType = RoomType.Scene;
            this.roomId = roomInfo.roomId;
            this.roomInfo = roomInfo;
        }

        public SceneRoomInfo? lastSceneRoomInfo;
        public override void OnAddedToDict()
        {
            // 有值就不能再赋值了，不然玩家上线下线就错了
            MyDebug.Assert(this.lastSceneRoomInfo == null);

            this.lastSceneRoomInfo = SceneRoomInfo.Ensure(null);
            this.lastSceneRoomInfo.DeepCopyFrom(this.roomInfo);
        }

        public Dictionary<long, SceneRoomUser> userDict = new();
        public int userCount
        {
            get
            {
                return this.userDict.Count;
            }
        }
        public SceneRoomUser? GetUser(long userId)
        {
            return this.userDict.TryGetValue(userId, out SceneRoomUser? user) ? user : null;
        }
        public bool RemoveUser(long userId)
        {
            return this.userDict.Remove(userId);
        }
        public void AddUser(SceneRoomUser user)
        {
            this.userDict.Add(user.userId, user);
        }
        public readonly Queue<ChatMessage> recentMessages = new();
    }
}