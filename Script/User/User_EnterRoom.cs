using Data;

namespace Script
{
    public class User_EnterRoom : User_ClientHandler<MsgEnterRoom, ResEnterRoom>
    {
        public override MsgType msgType => MsgType.EnterRoom;
        public User_EnterRoom(Server server, UserService service) : base(server, service)
        {
        }

        protected override Task<ECode> Handle(UserConnection connection, MsgEnterRoom msg, ResEnterRoom res)
        {
            User user = connection.user;
            if (user.roomId != 0)
            {
                // exit first
                
            }

            throw new NotImplementedException();
        }
    }
}