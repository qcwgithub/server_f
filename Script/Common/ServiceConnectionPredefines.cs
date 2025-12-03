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

        async Task<MyResponse> SendToServiceAsync(ServiceType serviceType, MsgType type, object msg)
        {
            ProtocolClientData socket = this.self.tcpClientScript.RandomOtherServiceSocket(serviceType);
            if (socket == null)
            {
                return ECode.Server_NotConnected;
            }
            MyResponse r = await socket.SendAsync(type, msg, pTimeoutS: null);
            if (r.err == ECode.Server_Timeout)
            {
                this.self.logger.ErrorFormat("send {0} to {1} Timeout", type.ToString(), socket.serviceTypeAndId.Value.ToString());
            }

            return r;
        }

        public async Task<MyResponse> SendAsync(MsgType msgType, object msg)
        {
            return await this.SendToServiceAsync(this.to, msgType, msg);
        }
    }

    public interface IConnectToDBService
    {
        Task<MyResponse> SendAsync(MsgType msgType, object msg);
    }

    public interface IConnectToDatabaseService
    {
        ConnectToDatabaseService connectToDatabaseService { get; }
    }

    public class ConnectToDatabaseService : ConnectToOtherService, IConnectToDBService
    {
        public ConnectToDatabaseService(Service self) : base(self, ServiceType.Database)
        {

        }
    }

    public class ConnectToGlobalService : ConnectToOtherService
    {
        public ConnectToGlobalService(Service self) : base(self, ServiceType.Global)
        {

        }
    }

    public class ConnectFromUserService
    {
        // 只能是 NormalService，因为 GroupService 虽然可以被 Player 连接，但是他不知道某个 playerId 属于哪个 psId，只能通过 GAAA -> AAA 转发
        Service self;
        public ConnectFromUserService(Service self)
        {
            this.self = self;

            // 必须是 User 有连接他的
            MyDebug.Assert(UserServiceData.s_connectToServiceIds.Contains(self.data.serviceType));
        }

        // 发送给 PlayerService 必须指定 serviceId
        public async Task<MyResponse> SendAsync(int psId, MsgType msgType, object msg)
        {
            ProtocolClientData socket = this.self.data.GetOtherServiceSocket(psId);
            if (socket == null || !socket.IsConnected())
            {
                return ECode.Server_NotConnected;
            }
            return await socket.SendAsync(msgType, msg, pTimeoutS: null);
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
            this.self.Dispatch(null, /* seq */0, type, msg, (e, r) =>
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