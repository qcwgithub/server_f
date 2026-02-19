using Data;

namespace Script
{
    public class FriendScript : ServiceScript<UserService>
    {
        public FriendScript(Server server, UserService service) : base(server, service)
        {
        }

        public FriendInfo DoAddFriend(UserInfo userInfo, long friendUserId, long timeS, long privateRoomId)
        {
            // 1
            int friendIndex = userInfo.friends.FindIndex(x => x.userId == friendUserId);
            FriendInfo friendInfo;
            if (friendIndex >= 0)
            {
                friendInfo = userInfo.friends[friendIndex];
                friendInfo.privateRoomId = privateRoomId;
            }
            else
            {
                friendInfo = FriendInfo.Ensure(null);
                friendInfo.userId = friendUserId;
                userInfo.friends.Add(friendInfo);
            }
            friendInfo.timeS = timeS;
            friendInfo.privateRoomId = privateRoomId;

            // 2
            int removedFriendIndex = userInfo.removedFriends.FindIndex(x => x.userId == friendUserId);
            if (removedFriendIndex >= 0)
            {
                userInfo.removedFriends.RemoveAt(removedFriendIndex);
            }

            return friendInfo;
        }

        public FriendInfo DoRemoveFriend(UserInfo userInfo, long friendUserId, int friendIndex, long timeS)
        {
            FriendInfo friendInfo = userInfo.friends[friendIndex];

            // 1
            userInfo.friends.RemoveAt(friendIndex);

            // 2
            var removedFriendInfo = FriendInfo.Ensure(null);
            removedFriendInfo.DeepCopyFrom(friendInfo);
            removedFriendInfo.timeS = timeS;

            int removedFriendIndex = userInfo.removedFriends.FindIndex(x => x.userId == friendUserId);
            if (removedFriendIndex >= 0)
            {
                userInfo.removedFriends[removedFriendIndex] = removedFriendInfo;
            }
            else
            {
                userInfo.removedFriends.Add(removedFriendInfo);
            }

            return removedFriendInfo;
        }
    }
}