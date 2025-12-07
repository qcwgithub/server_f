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

        async Task<MyResponse> SendToServiceAsync<T>(ServiceType serviceType, MsgType type, T msg)
        {
            ProtocolClientData socket = this.self.tcpClientScript.RandomOtherServiceSocket(serviceType);
            if (socket == null)
            {
                return ECode.Server_NotConnected;
            }

            byte[] bytes = this.self.server.messageSerializer.Serialize<T>(msg);
            MyResponse r = await socket.SendAsync(type, bytes, pTimeoutS: null);
            if (r.err == ECode.Server_Timeout)
            {
                this.self.logger.ErrorFormat("send {0} to {1} Timeout", type.ToString(), socket.serviceTypeAndId.Value.ToString());
            }

            return r;
        }

        public async Task<MyResponse> SendAsync<T>(MsgType msgType, T msg)
        {
            return await this.SendToServiceAsync(this.to, msgType, msg);
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
        public async Task<MyResponse> SendAsync<T>(int serviceId, MsgType msgType, T msg)
        {
            ProtocolClientData socket = this.self.data.GetOtherServiceSocket(serviceId);
            if (socket == null || !socket.IsConnected())
            {
                return ECode.Server_NotConnected;
            }

            byte[] bytes = this.self.server.messageSerializer.Serialize<T>(msg);
            return await socket.SendAsync(msgType, bytes, pTimeoutS: null);
        }

        public async Task<MyResponse> SendToAllAsync(MsgType msgType, object msg)
        {
            return await this.self.tcpClientScript.SendToAllServiceAsync(ServiceType.User, msgType, msg);
        }

        public async Task<List<MyResponse>> SendToAllAsync2(MsgType msgType, object msg)
        {
            return await this.self.tcpClientScript.SendToAllServiceAsync2(ServiceType.User, msgType, msg);
        }
    }

    public class MonitorConnectToSameServerType
    {
        Service self;
        public MonitorConnectToSameServerType(Service self)
        {
            this.self = self;
        }

        public async Task<MyResponse> SendToServiceAsync<T>(int serviceId, MsgType msgType, T msg)
        {
            ProtocolClientData socket = this.self.data.GetOtherServiceSocket(serviceId);
            if (socket == null || !socket.IsConnected())
            {
                return ECode.Server_NotConnected;
            }

            byte[] bytes = this.self.server.messageSerializer.Serialize<T>(msg);
            return await socket.SendAsync(msgType, bytes, pTimeoutS: null);
        }
    }

    public class ConnectToSelf
    {
        Service self;
        public ConnectToSelf(Service self)
        {
            this.self = self;
        }

        public async Task<MyResponse> SendToSelfAsync(MsgType type, object msg)
        {
            var cs = new TaskCompletionSource<MyResponse>();
            this.self.dispatcher.Dispatch(null, type, msg, (e, r) =>
            {
                bool success = cs.TrySetResult(new MyResponse(e, r));
                if (!success)
                {
                    Console.WriteLine("!cs.TrySetResult " + type);
                }
            });
            var xxx = await cs.Task;
            return xxx;
        }
    }
}