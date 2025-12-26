using System.Threading.Tasks;
using Data;
namespace Script
{
    // 其他服 -> User
    public class User_SaveUserImmediately : User_ServerHandler<MsgSaveUser, ResSaveUser>
    {
        public User_SaveUserImmediately(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_SaveUserImmediately;

        protected override async Task<ECode> Handle(ServiceConnection connection, MsgSaveUser msg, ResSaveUser res)
        {
            var r = await this.service.connectToSelf.Request<MsgSaveUser, ResSaveUser>(MsgType._User_SaveUser, msg);
            return r.e;
        }
    }
}