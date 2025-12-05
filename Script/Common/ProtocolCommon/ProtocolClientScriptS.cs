using System;
using Data;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        public void Dispatch(ProtocolClientData data, MsgType msgType, ArraySegment<byte> msg, Action<ECode, byte[]> reply)
        {
            this.service.dispatcher.Dispatch(data, msgType, msg, reply);
        }

        public void OnConnectComplete(ProtocolClientData socket, bool success)
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
            this.service.dispatcher.Dispatch(socket, MsgType._OnConnectComplete, msg, null);
        }

        public void OnCloseComplete(ProtocolClientData socket)
        {
            var msg = new MsgOnClose
            {
                isAcceptor = !socket.isConnector,
                // isServer = @this.connectedFromServer,
            };
            this.service.dispatcher.Dispatch(socket, MsgType._OnSocketClose, msg, null);
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
        public async Task<MyResponse> SendToAllServiceAsync(ServiceType serviceType, MsgType type, object msg)
        {
            List<ProtocolClientData> list = this.service.data.otherServiceSockets2[(int)serviceType];
            if (list == null || list.Count == 0)
            {
                return ECode.Server_NotConnected;
            }

            MyResponse r = null;
            ProtocolClientData[] copy = list.ToArray();

            foreach (var socket in copy)
            {
                if (socket != null && socket.IsConnected())
                {
                    r = await socket.SendAsync(type, msg, pTimeoutS: null);
                    if (r.err == ECode.Server_Timeout)
                    {
                        this.service.logger.ErrorFormat("send {0} to {1} Timeout", type.ToString(), socket.serviceTypeAndId.Value.ToString());
                    }
                }
            }

            if (r == null)
            {
                return ECode.Server_NotConnected;
            }
            else
            {
                // 返回最后一个发送结果
                return r;
            }
        }

        // 根据 服务类型， 向 全部这个类型的服务 统一发送
        public async Task<List<MyResponse>> SendToAllServiceAsync2(ServiceType serviceType, MsgType type, object msg)
        {
            var responses = new List<MyResponse>();

            List<ProtocolClientData> list = this.service.data.otherServiceSockets2[(int)serviceType];
            if (list == null || list.Count == 0)
            {
                return responses;
            }

            ProtocolClientData[] copy = list.ToArray();

            foreach (var socket in copy)
            {
                if (socket != null && socket.IsConnected())
                {
                    MyResponse r = await socket.SendAsync(type, msg, pTimeoutS: null);
                    responses.Add(r);
                    if (r.err == ECode.Server_Timeout)
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