using Data;
using DnsClient.Protocol;

namespace Script
{
    public class ProtocolClientScriptS : ServiceScript<Service>, IProtocolClientCallback
    {
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

        public async void OnConnectComplete(ProtocolClientData socket, bool success)
        {
            if (!success)
            {
                socket.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                return;
            }

            MyDebug.Assert(socket.serviceTypeAndId != null);
            var serviceTypeAndId = socket.serviceTypeAndId.Value;

            var msg = new MsgOnConnectComplete();
            msg.to_serviceType = serviceTypeAndId.serviceType;
            msg.to_serviceId = serviceTypeAndId.serviceId;
            await this.service.dispatcher.DispatchLocal<MsgOnConnectComplete, ResOnConnectComplete>(socket, MsgType._OnConnectComplete, msg);
        }

        public async void OnCloseComplete(ProtocolClientData socket)
        {
            var msg = new MsgSocketClose
            {
                isAcceptor = !socket.isConnector,
                // isServer = @this.connectedFromServer,
            };
            await this.service.dispatcher.DispatchLocal<MsgSocketClose, ResSocketClose>(socket, MsgType._OnSocketClose, msg);
        }

        #region basic access

        public bool IsServiceConnected(int serviceId)
        {
            ProtocolClientData? socket;
            if (!this.service.data.otherServiceSockets.TryGetValue(serviceId, out socket) || !socket.IsConnected())
            {
                return false;
            }
            return true;
        }
        #endregion

        #region bind user

        public void BindUser(ProtocolClientData @this, User user)
        {
            if (!user.IsRealPrepareLogin(out MsgPrepareUserLogin msgPreparePlayerLogin))
            {
                MyDebug.Assert(false);
            }

            user.socket = @this;
            @this.user = user;
            @this.userId = user.userId;
            @this.user_version = msgPreparePlayerLogin.version;
            @this.lastUserId = user.userId;
        }

        public void UnbindUser(ProtocolClientData @this, User user)
        {
            user.socket = null;
            @this.user = null;
            @this.userId = 0;
            @this.user_version = string.Empty;
        }

        public object? GetUser(ProtocolClientData @this)
        {
            return @this.user == null ? null : @this.user;
        }


        #endregion

        #region send

        public ProtocolClientData RandomOtherServiceSocket(ServiceType serviceType)
        {
            List<ProtocolClientData> list = this.service.data.otherServiceSockets2[(int)serviceType];
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
            List<ProtocolClientData> list = this.service.data.otherServiceSockets2[(int)serviceType];
            if (list == null || list.Count == 0)
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }

            ProtocolClientData[] copy = list.ToArray();

            byte[] bytes = this.server.messageSerializer.Serialize<Msg>(msg);
            MyResponse<Res> r = null;

            foreach (var socket in copy)
            {
                if (socket != null && socket.IsConnected())
                {
                    r = await socket.Request<Msg, Res>(type, bytes);
                    if (r.e == ECode.Server_Timeout)
                    {
                        this.service.logger.ErrorFormat("send {0} to {1} Timeout", type.ToString(), socket.serviceTypeAndId.Value.ToString());
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

            List<ProtocolClientData> list = this.service.data.otherServiceSockets2[(int)serviceType];
            if (list == null || list.Count == 0)
            {
                return responses;
            }

            ProtocolClientData[] copy = list.ToArray();

            byte[] bytes = this.server.messageSerializer.Serialize<Msg>(msg);

            foreach (var socket in copy)
            {
                if (socket != null && socket.IsConnected())
                {
                    var r = await socket.Request<Msg, Res>(type, bytes);
                    responses.Add(r);
                    if (r.e == ECode.Server_Timeout)
                    {
                        this.service.logger.ErrorFormat("send {0} to {1} Timeout", type.ToString(), socket.serviceTypeAndId.Value.ToString());
                    }
                }
            }

            return responses;
        }

        #endregion
    }
}