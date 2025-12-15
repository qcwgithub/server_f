using Data;

namespace Script
{
    public class ProtocolClientScriptForS : ProtocolClientScript
    {
        public ProtocolClientScriptForS(Server server, Service service) : base(server, service)
        {
        }

        public ProtocolClientData CreateConnector(IProtocolClientCallbackProvider callbackProvider, string ip, int port)
        {
            var socket = new TcpClientData();
            socket.ConnectorInit(callbackProvider, ip, port);
            return socket;
        }

        public bool IsServiceConnected(int serviceId)
        {
            ServiceConnection? connection;
            if (!this.service.data.otherServiceConnections.TryGetValue(serviceId, out connection) || !connection.IsConnected())
            {
                return false;
            }
            return true;
        }

        public ServiceConnection? RandomOtherServiceConnection(ServiceType serviceType)
        {
            List<ServiceConnection> list = this.service.data.otherServiceConnections2[(int)serviceType];
            if (list == null || list.Count == 0)
            {
                return null;
            }

            int index = SCUtils.WeightedRandomSimple(this.service.data.random, list.Count, i =>
            {
                if (list[i].IsConnected())
                {
                    return 1;
                }
                return 0;
            });

            if (index == -1)
            {
                return null;
            }
            return list[index];
        }

        // 根据 服务类型， 向 全部这个类型的服务 统一发送
        public async Task<MyResponse<Res>> SendToAllService<Msg, Res>(ServiceType serviceType, MsgType type, Msg msg)
            where Res : class
        {
            List<ServiceConnection> list = this.service.data.otherServiceConnections2[(int)serviceType];
            if (list == null || list.Count == 0)
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }

            ServiceConnection[] copy = list.ToArray();

            byte[] bytes = this.server.messageSerializer.Serialize<Msg>(msg);
            MyResponse<Res>? r = null;

            foreach (var connection in copy)
            {
                if (connection != null && connection.IsConnected())
                {
                    r = await connection.Request<Msg, Res>(type, bytes);
                    if (r.e == ECode.Server_Timeout)
                    {
                        this.service.logger.ErrorFormat("send {0} to {1} Timeout", type.ToString(), connection.serviceTypeAndId.Value.ToString());
                    }
                }
            }

            if (r == null)
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }
            else
            {
                // 返回最后一个发送结果
                return r;
            }
        }

        // 根据 服务类型， 向 全部这个类型的服务 统一发送
        public async Task<List<MyResponse<Res>>> SendToAllServiceAsync2<Msg, Res>(ServiceType serviceType, MsgType type, Msg msg)
            where Res : class
        {
            var responses = new List<MyResponse<Res>>();

            List<ServiceConnection> list = this.service.data.otherServiceConnections2[(int)serviceType];
            if (list == null || list.Count == 0)
            {
                return responses;
            }

            ServiceConnection[] copy = list.ToArray();

            byte[] bytes = this.server.messageSerializer.Serialize<Msg>(msg);

            foreach (var connection in copy)
            {
                if (connection != null && connection.IsConnected())
                {
                    var r = await connection.Request<Msg, Res>(type, bytes);
                    responses.Add(r);
                    if (r.e == ECode.Server_Timeout)
                    {
                        this.service.logger.ErrorFormat("send {0} to {1} Timeout", type.ToString(), connection.serviceTypeAndId.Value.ToString());
                    }
                }
            }

            return responses;
        }
    }
}