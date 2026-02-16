using Data;

namespace Script
{
    public class FriendScript : ServiceScript<UserService>
    {
        public FriendScript(Server server, UserService service) : base(server, service)
        {
        }

        public bool IsMyFriend(User user, long targetUserId)
        {
            return user.userInfo.friends.Exists(info => info.userId == targetUserId);
        }
    }
}