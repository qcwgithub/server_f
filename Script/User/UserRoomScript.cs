using Data;

namespace Script
{
    public class UserRoomScript : ServiceScript<UserService>
    {
        public UserRoomScript(Server server, UserService service) : base(server, service)
        {
        }

        public ECode CheckSendSceneChat(User user, MsgSendSceneChat msg)
        {
            if (msg.roomId != user.publicRoomId)
            {
                return ECode.WrongRoomId;
            }
            return ECode.Success;
        }

        public ECode CheckSendPrivateChat(User user, MsgSendPrivateChat msg)
        {
            UserInfo userInfo = user.userInfo;

            int friendIndex = userInfo.friends.FindIndex(x => x.userId == msg.friendUserId);
            if (friendIndex < 0)
            {
                return ECode.NotFriends;
            }

            return ECode.Success;
        }
    }
}