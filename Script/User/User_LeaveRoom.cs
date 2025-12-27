using Data;

namespace Script
{
    public class User_LeaveRoom : User_ClientHandler<MsgLeaveRoom, ResLeaveRoom>
    {
        public override MsgType msgType => MsgType.LeaveRoom;
        public User_LeaveRoom(Server server, UserService service) : base(server, service)
        {
        }

        protected override Task<ECode> Handle(UserConnection connection, MsgLeaveRoom msg, ResLeaveRoom res)
        {
            throw new NotImplementedException();
        }
    }
}