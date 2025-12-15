using Data;

namespace Script
{
    public abstract class ConnectToOtherService
    {
        public Service self { get; private set; }
        public ServiceType to { get; private set; }
        public ConnectToOtherService(Service self, ServiceType to)
        {
            this.self = self;
            this.to = to;

            // 重点
            // 要发送给他，必须有定义连接他才行
            MyDebug.Assert(self.data.connectToServiceTypes.Contains(to));
        }

        async Task<MyResponse<Res>> Request<Msg, Res>(ServiceType serviceType, MsgType type, Msg msg)
            where Res : class
        {
            IConnection? connection = this.self.protocolClientScriptForS.RandomOtherServiceConnection(serviceType);
            if (connection == null)
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }

            return await connection.Request<Msg, Res>(type, msg);
        }

        public async Task<MyResponse<Res>> Request<Msg, Res>(MsgType msgType, Msg msg)
            where Res : class
        {
            return await this.Request<Msg, Res>(this.to, msgType, msg);
        }
    }

    public class ConnectToDbService : ConnectToOtherService
    {
        public ConnectToDbService(Service self) : base(self, ServiceType.Db)
        {

        }
    }

    public class ConnectToGlobalService : ConnectToOtherService
    {
        public ConnectToGlobalService(Service self) : base(self, ServiceType.Global)
        {

        }
    }

    public class ConnectToUserManagerService : ConnectToOtherService
    {
        public ConnectToUserManagerService(Service self) : base(self, ServiceType.UserManager)
        {

        }
    }

    public class ConnectToGatewayService : ConnectToOtherService
    {
        public ConnectToGatewayService(Service self) : base(self, ServiceType.Gateway)
        {

        }
    }

    public class ConnectFromUserService
    {
        Service self;
        public ConnectFromUserService(Service self)
        {
            this.self = self;

            // 必须是 User 有连接他的
            MyDebug.Assert(UserServiceData.s_connectToServiceIds.Contains(self.data.serviceType));
        }

        // 发送给 UserService 必须指定 serviceId
        public async Task<MyResponse<Res>> Send<Msg, Res>(int serviceId, MsgType msgType, Msg msg)
            where Res : class
        {
            IConnection? connection = this.self.data.GetOtherServiceConnection(serviceId);
            if (connection == null || !connection.IsConnected())
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }

            return await connection.Request<Msg, Res>(msgType, msg);
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

    public class MonitorConnectToSameServerType
    {
        Service self;
        public MonitorConnectToSameServerType(Service self)
        {
            this.self = self;
        }

        public async Task<MyResponse<Res>> Request<Msg, Res>(int serviceId, MsgType msgType, Msg msg)
            where Res : class, new()
        {
            IConnection? connection = this.self.data.GetOtherServiceConnection(serviceId);
            if (connection == null || !connection.IsConnected())
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }

            return await connection.Request<Msg, Res>(msgType, msg);
        }
    }

    public class ConnectToSelf
    {
        Service self;
        public ConnectToSelf(Service self)
        {
            this.self = self;
        }

        public async Task<MyResponse<Res>> Request<Msg, Res>(MsgType msgType, Msg msg)
            where Res : class
        {
            return await this.self.dispatcher.Dispatch<Msg, Res>(this.self.data.localConnection, msgType, msg);
        }
    }
}