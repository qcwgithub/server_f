namespace Data
{
    public class FriendChatRoom : Room
    {
        public readonly FriendChatInfo friendChatInfo;
        public FriendChatRoom(FriendChatInfo friendChatInfo)
        {
            this.roomType = RoomType.Private;
            this.roomId = friendChatInfo.roomId;
            this.friendChatInfo = friendChatInfo;
        }

        public long GetOtherUserId(long userId)
        {
            return this.friendChatInfo.users[0].userId == userId
                ? this.friendChatInfo.users[1].userId
                : this.friendChatInfo.users[0].userId;
        }

        public FriendChatInfo? lastFriendChatInfo;
        public override void OnAddedToDict()
        {
            // 有值就不能再赋值了，不然玩家上线下线就错了
            MyDebug.Assert(this.lastFriendChatInfo == null);

            this.lastFriendChatInfo = FriendChatInfo.Ensure(null);
            this.lastFriendChatInfo.DeepCopyFrom(this.friendChatInfo);
        }

        public readonly List<ChatMessage> recentMessages = new();
    }
}