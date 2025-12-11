using Data;
using DnsClient.Protocol;

namespace Script
{
    public class ProtocolClientScriptS : ServiceScript<Service>, IProtocolClientCallback
    {
        public ProtocolClientScriptS(Server server, Service service) : base(server, service)
        {
        }

        public IMessagePacker GetMessagePacker()
        {
            return this.server.messagePacker;
        }

        public void LogError(ProtocolClientData data, string str)
        {
            if (data.userId > 0)
            {
                this.service.logger.Error($"userId ({data.userId}) version ({data.user_version}) {str}");
            }
            else
            {
                this.service.logger.Error(str);
            }
        }

        public void LogError(ProtocolClientData data, string str, Exception ex)
        {
            if (data.userId > 0)
            {
                this.service.logger.Error($"userId ({data.userId}) version ({data.user_version}) {str}", ex);
            }
            else
            {
                this.service.logger.Error(str, ex);
            }
        }

        public void LogInfo(ProtocolClientData data, string str)
        {
            this.service.logger.Info(str);
        }

        public int nextSocketId
        {
            get
            {
                return this.service.data.socketId++;
            }
        }

        public int nextMsgSeq
        {
            get
            {
                return this.service.data.msgSeq++;
            }
        }

        public async void DispatchNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, Action<ECode, byte[]>? reply)
        {
            (ECode e, byte[] resBytes) = await this.service.dispatcher.DispatchNetwork(data, msgType, msgBytes);
            if (reply != null)
            {
                reply(e, resBytes);
            }
        }

        public async void OnConnectComplete(IConnection connection, bool success)
        {
            if (!success)
            {
                connection.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                return;
            }

            MyDebug.Assert(socket.serviceTypeAndId != null);
            var serviceTypeAndId = connection.serviceTypeAndId.Value;

            var msg = new MsgOnConnectComplete();
            msg.to_serviceType = serviceTypeAndId.serviceType;
            msg.to_serviceId = serviceTypeAndId.serviceId;
            await this.service.dispatcher.DispatchLocal<MsgOnConnectComplete, ResOnConnectComplete>(connection, MsgType._OnConnectComplete, msg);
        }

        public async void OnCloseComplete(IConnection connection)
        {
            var msg = new MsgConnectionClose
            {
                isAcceptor = !connection.isConnector,
                // isServer = @this.connectedFromServer,
            };
            await this.service.dispatcher.DispatchLocal<MsgConnectionClose, ResConnectionClose>(connection, MsgType._OnConnectionClose, msg);
        }

        #region basic access

        public bool IsServiceConnected(int serviceId)
        {
            ServiceConnection? connection;
            if (!this.service.data.otherServiceConnections.TryGetValue(serviceId, out connection) || !connection.IsConnected())
            {
                return false;
            }
            return true;
        }
        #endregion

        #region send

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

        #endregion
    }
}