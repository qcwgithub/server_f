using Data;

namespace Script
{
    public class ConnectFromUserService : ConnectWithUserService
    {
        public ConnectFromUserService(Service self) : base(self, ServiceType.User)
        {
            // 必须是 User 有连接他的
            MyDebug.Assert(UserServiceData.s_connectToServiceIds.Contains(self.data.serviceType));
        }
    }
}