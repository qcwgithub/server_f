using System;
using Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Script
{
    public class ProtocolClientScriptS : ServiceScript<BaseServer, BaseService>, IProtocolClientCallback
    {
        public IMessagePacker GetMessagePacker(bool isMessagePack)
        {
            if (isMessagePack)
                return this.service.messagePackerBin;
            else
                return this.service.messagePackerJson;
        }

        public void LogError(ProtocolClientData data, string str)
        {
            if (data.playerId > 0)
            {
                this.service.logger.Error($"playerId ({data.playerId}) version ({data.player_version}) {str}");
            }
            else
            {
                this.service.logger.Error(str);
            }
        }

        public void LogError(ProtocolClientData data, string str, Exception ex)
        {
            if (data.playerId > 0)
            {
                this.service.logger.Error($"playerId ({data.playerId}) version ({data.player_version}) {str}", ex);
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

        public void Dispatch(ProtocolClientData data, int seq, MsgType msgType, object msg, Action<ECode, object> reply)
        {
            this.service.Dispatch(data, seq, msgType, msg, reply);
        }

        public void OnConnectComplete(ProtocolClientData data, bool success)
        {
            if (!success)
            {
                data.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                return;
            }

            MyDebug.Assert(data.serviceTypeAndId != null);
            var serviceTypeAndId = data.serviceTypeAndId.Value;

            var msg = new MsgOnConnectComplete();
            msg.to_serviceType = serviceTypeAndId.serviceType;
            msg.to_serviceId = serviceTypeAndId.serviceId;
            this.service.Dispatch(data, 0, MsgType._OnConnectComplete, msg, null);
        }

        public void OnCloseComplete(ProtocolClientData data)
        {
            var msg = new MsgOnClose
            {
                isAcceptor = !data.isConnector,
                // isServer = @this.connectedFromServer,
            };
            this.service.Dispatch(data, 0, MsgType._OnSocketClose, msg, null);
        }

        #region basic access

        public bool IsServiceConnected(int serviceId)
        {
            ProtocolClientData socket;
            if (!this.service.data.otherServiceSockets.TryGetValue(serviceId, out socket) || !socket.IsConnected())
            {
                return false;
            }
            return true;
        }
        #endregion

        #region bind player

        public void BindPlayer(ProtocolClientData @this, PSPlayer player)
        {
            if (!player.IsRealPrepareLogin(out MsgPreparePlayerLogin msgPreparePlayerLogin))
            {
                MyDebug.Assert(false);
            }

            player.socket = @this;
            @this.player = player;
            @this.playerId = player.playerId;
            @this.player_version = msgPreparePlayerLogin.version;
            @this.lastPlayerId = player.playerId;
        }

        public void UnbindPlayer(ProtocolClientData @this, PSPlayer player)
        {
            player.socket = null;
            @this.player = null;
            @this.playerId = 0;
            @this.player_version = string.Empty;
        }

        public object GetPlayer(ProtocolClientData @this)
        {
            return @this.player == null ? null : @this.player;
        }


        #endregion

        #region send

        public ProtocolClientData RandomOtherServiceSocket(ServiceType serviceType)
        {
            this.service.data.serviceType.AssertIsSameServerType(serviceType, "RandomOtherServiceSocket", this.service.logger);

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
            this.service.data.serviceType.AssertIsSameServerType(serviceType, "SendToAllServiceAsync", this.service.logger);

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
            this.service.data.serviceType.AssertIsSameServerType(serviceType, "SendToAllServiceAsync2", this.service.logger);

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