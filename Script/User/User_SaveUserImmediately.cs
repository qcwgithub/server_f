using System.Threading.Tasks;
using Data;
namespace Script
{
    // 其他服 -> User
    public class User_SaveUserImmediately : Handler<UserService>
    {
        public override MsgType msgType => MsgType._User_SaveUserImmediately;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSaveUser>(_msg);
            MyResponse r = await this.service.connectToSelf.SendToSelfAsync(MsgType._User_SaveUser, msg);
            return r;
        }
    }
}