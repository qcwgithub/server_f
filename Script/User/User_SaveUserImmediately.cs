using System.Threading.Tasks;
using Data;
namespace Script
{
    // 其他服 -> User
    public class User_SaveUserImmediately : UserHandler<MsgSaveUserImmediately, ResSaveUserImmediately>
    {
        public User_SaveUserImmediately(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_SaveUserImmediately;

        public override async Task<ECode> Handle(MessageContext context, MsgSaveUserImmediately msg, ResSaveUserImmediately res)
        {
            ECode e = await this.service.SaveUser(msg.userId, msg.reason);
            return e;
        }
    }
}