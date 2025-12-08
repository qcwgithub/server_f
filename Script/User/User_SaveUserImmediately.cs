using System.Threading.Tasks;
using Data;
namespace Script
{
    // 其他服 -> User
    public class User_SaveUserImmediately : UserHandler<MsgSaveUser, ResSaveUser>
    {
        public override MsgType msgType => MsgType._User_SaveUserImmediately;

        public override async Task<ECode> Handle(ProtocolClientData socket, MsgSaveUser msg, ResSaveUser res)
        {
            MyResponse r = await this.service.connectToSelf.SendToSelfAsync(MsgType._User_SaveUser, msg);
            return r;
        }
    }
}