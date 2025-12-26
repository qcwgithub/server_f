using Data;

namespace Script
{
    public class ConnectFromUserService : ConnectWithUserService
    {
        public ConnectFromUserService(UserService self) : base(self)
        {
            // 必须是 User 有连接他的
            MyDebug.Assert(UserServiceData.s_connectToServiceIds.Contains(self.data.serviceType));
        }

        public async Task<MyResponse<Res>> SendToAll<Msg, Res>(MsgType msgType, Msg msg) where Res : class
        {
            return await this.self.protocolClientScriptForS.SendToAllService<Msg, Res>(ServiceType.User, msgType, msg);
        }

        public async Task<List<MyResponse<Res>>> SendToAll2<Msg, Res>(MsgType msgType, Msg msg) where Res : class
        {
            return await this.self.protocolClientScriptForS.SendToAllServiceAsync2<Msg, Res>(ServiceType.User, msgType, msg);
        }
    }
}