namespace Data
{
    public class FriendChatRoom : Room
    {
        public readonly FriendChatRoomInfo friendChatRoomInfo;
        public FriendChatRoom(FriendChatRoomInfo friendChatRoomInfo)
        {
            this.roomType = RoomType.Private;
            this.roomId = friendChatRoomInfo.roomId;
            this.friendChatRoomInfo = friendChatRoomInfo;
        }

        public long GetOtherUserId(long userId)
        {
            return this.friendChatRoomInfo.users[0].userId == userId
                ? this.friendChatRoomInfo.users[1].userId
                : this.friendChatRoomInfo.users[0].userId;
        }

        public FriendChatRoomInfo? lastFriendChatRoomInfo;
        public override void OnAddedToDict()
        {
            // 有值就不能再赋值了，不然玩家上线下线就错了
            MyDebug.Assert(this.lastFriendChatRoomInfo == null);

            this.lastFriendChatRoomInfo = FriendChatRoomInfo.Ensure(null);
            this.lastFriendChatRoomInfo.DeepCopyFrom(this.friendChatRoomInfo);
        }

        public readonly List<ChatMessage> recentMessages = new();
    }
}