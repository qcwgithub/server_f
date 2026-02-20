using Data;

namespace Script
{
    public class UserRoomScript : ServiceScript<UserService>
    {
        public UserRoomScript(Server server, UserService service) : base(server, service)
        {
        }

        public ECode CheckSendRoomChat(User user, MsgSendRoomChat msg)
        {
            UserInfo userInfo = user.userInfo;
            switch (msg.roomType)
            {
                case RoomType.Private:
                    {
                        int friendIndex = userInfo.friends.FindIndex(x => x.privateRoomId == msg.roomId);
                        if (friendIndex < 0)
                        {
                            return ECode.WrongRoomId;
                        }
                    }
                    break;
                case RoomType.Public:
                    {
                        if (msg.roomId != user.publicRoomId)
                        {
                            return ECode.WrongRoomId;
                        }
                    }
                    break;
                default:
                    throw new Exception($"Not handled roomType.{msg.roomType}");
            }
            return ECode.Success;
        }
    }
}