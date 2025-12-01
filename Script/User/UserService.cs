using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Script.Script;

namespace Script
{
    public class UserService : Service, IConnectToDatabaseService
    {
        //
        public ConnectToDatabaseService connectToDatabaseService { get; private set; }
        public ConnectToGlobalService connectToConfigManagerService { get; private set; }

        public PSData psData
        {
            get
            {
                return (PSData)this.data;
            }
        }
        public PSScript psScript;
        public PMSqlUtils pmSqlUtils;
        public PMScriptCreateNewPlayer pmScriptCreateNewPlayer;

        public UserService(Server server, int serviceId) : base(server, serviceId)
        {
        }

        protected override MessageDispatcher CreateMessageDispatcher(BaseServer baseScriptEntry)
        {
            return new PSMessageDispatcher().Init(baseScriptEntry, (BaseService)this);
        }

        public override void Attach()
        {
            base.Attach();

            //
            this.AddConnectToOtherService(this.connectToDatabaseService = new ConnectToDatabaseService(this));
            this.AddConnectToOtherService(this.connectToConfigManagerService = new ConnectToGlobalService(this));

            //
            var ltChannelCheckSensitive = new Leiting.ChannelCheckSensitive().Init(this.server, this);
            var wechatCheckSensitive = new Wechat.WeChatCheckSensitive().Init(this.server, this);
            var commonCheckSensitive = new CommonCheckSensitive().Init(this.server, this);
            this.checkSensitiveDic = new Dictionary<string, List<ICheckSensitive>>
            {
                {MyChannels.leiting_en, new List<ICheckSensitive>{ ltChannelCheckSensitive }},
                {MyChannels.leiting, new List<ICheckSensitive>{ ltChannelCheckSensitive }},
                {MyChannels.wechat, new List<ICheckSensitive>{ ltChannelCheckSensitive, wechatCheckSensitive }},
                {MyChannels.uuid, new List<ICheckSensitive>{ commonCheckSensitive }},
                {MyChannels.quick, new List<ICheckSensitive>{ commonCheckSensitive }},
            };

            this.playerNamesRedis = new PlayerNamesRedis().Init(this.server);
            this.clientPostHandleScript = new PlayerClientPostHandleScript().Init(this.server, this);
            this.autoRequest = new PlayerAutoRequest().Init(this.server, this);
            this.globalInfoCacheScript = new GlobalInfoCacheScript().Init(this.server, this);
            this.robotInfoCacheScript = new RobotInfoCacheScript().Init(this.server, this);
            this.originalMailCacheScript = new OriginalMailCacheScript().Init(this.server, this);
            this.playerBattleSideTemplateScript = new PlayerBattleSideTemplateScript().Init(this.server, this);
            this.playerBriefInfoTemplateScript = new PlayerBriefInfoTemplateScript().Init(this.server, this);
            this.worldMapCacheScript = new WorldMapCacheScript().Init(this.server, this);
            this.worldMapViewportRequester = new WorldMapViewportRequester().Init(this.server, this);
            this.worldMapInfoRequester = new WorldMapInfoRequester().Init(this.server, this);
            this.worldMapManager = new WorldMapManager().Init(this.server, this);
            this.arenaMatchRankingListRedis = new ArenaMatchRankingListRedis().Init(this.server);
            this.loopBattleTest = new LoopBattleTest().Init(this.server, this);
            this.replayClearedBattleTest = new ReplayClearedBattleTest().Init(this.server, this);

            base.AddHandler<PlayerService>();

            // 覆盖 OnConnectComplete
            this.dispatcher.AddHandler(new PlayerS_OnConnectComplete().Init(this.server, this), true);
#if DEBUG
            this.dispatcher.AddHandler(new PlayerS_OnSetTimeOffset().Init(this.server, this), true);
#endif

            this.psScript = new PSScript().Init(this.server, this);
            this.pmSqlUtils = new PMSqlUtils().Init(this.server, this);
            this.pmScriptCreateNewPlayer = new PMScriptCreateNewPlayer().Init(this.server, this);
            this.taScript = new TAScript().Init(this.server);
        }

        public override async Task Detach()
        {
            await base.Detach();
        }

        // PlayerService 在此对 reply 再包一层
        public override void Dispatch(ProtocolClientData data, int seq, MsgType msgType, object msg, Action<ECode, object> reply)
        {
            if (data != null && data.oppositeIsClient && msgType >= MsgType.ClientStart)
            {
                PSPlayer player = this.tcpClientScript.GetPlayer(data) as PSPlayer;
                if (player != null)
                {
                    base.Dispatch(data, seq, msgType, msg, (e, res) =>
                    {
                        // 额外处理
                        object resFinal = this.clientPostHandleScript.PostHandle(player, msgType, msg, e, res);

                        if (player.recentResList.Count > 0 && seq < player.recentResList[player.recentResList.Count - 1].seq)
                        {
                            player.recentResList.Clear();
                            this.logger.Debug($"recent res Clear()");
                        }

                        if (msgType.NeedRestoreRes())
                        {
                            player.recentResList.Add(new stRecentRes
                            {
                                seq = seq,
                                msgType = msgType,
                                e = e,
                                res = resFinal,
                            });

                            string res_s = resFinal == null ? "null" : resFinal.GetType().Name;
                            this.logger.Debug($"recent res + seq {seq} msgType {msgType} e {e} res {res_s}");

                            while (player.recentResList.Count > 20)
                            {
                                player.recentResList.RemoveAt(0);
                            }
                        }

                        reply(e, resFinal);
                    });

                    //
                    return;
                }
            }

            base.Dispatch(data, seq, msgType, msg, reply);
        }

        public List<RestoreRes> CalcRestoreResList(PSPlayer player)
        {
            try
            {
                if (player.recentResList.Count == 0)
                {
                    return null;
                }

                var restoreResList = new List<RestoreRes>();

                foreach (stRecentRes recent in player.recentResList)
                {
                    var restore = new RestoreRes();
                    restore.seq = recent.seq;
                    restore.msgType = recent.msgType;
                    restore.e = recent.e;

                    if (recent.res != null && recent.res is List<object> objects)
                    {
                        restore.isList = true;

                        restore.messageCodes = new List<MessageCode>();
                        restore.messageBins = new List<byte[]>();
                        foreach (object obj in objects)
                        {
                            MessageCode messageCode = TypeToMessageCodeCache.getMessageCode(obj);
                            restore.messageCodes.Add(messageCode);
                            restore.messageBins.Add(BinaryMessagePacker.PackBody(messageCode, obj));
                        }
                    }
                    else
                    {
                        restore.isList = false;

                        restore.messageCode = TypeToMessageCodeCache.getMessageCode(recent.res);
                        restore.messageBin = BinaryMessagePacker.PackBody(restore.messageCode, recent.res);
                    }

                    restoreResList.Add(restore);
                }

                return restoreResList;
            }
            catch (Exception ex)
            {
                this.logger.Error("CalcRestoreResList exception " + ex);
                return null;
            }
        }

        public async Task SendPSInfoToAAA(bool all, ProtocolClientData socket)
        {
            var serviceConfig = this.psData.serviceConfig;

            var psInfo = new PSInfo();
            psInfo.serviceId = this.serviceId;
            psInfo.playerCount = this.psData.playerDict.Count;
            psInfo.allowNewPlayer = this.psData.allowNewPlayer;

            //
            psInfo.outIp = serviceConfig.outIp;
            psInfo.outPort = serviceConfig.outPort;

            //
            psInfo.ws_outIp = serviceConfig.ws_outIp;
            psInfo.ws_outPort = serviceConfig.ws_outPort;

            var msgA = new MsgPSInfo();
            msgA.psInfo = psInfo;

            if (all)
            {
                await this.connectToStatelessService.SendToAllAsync(MsgType._AAA_PSInfo, msgA);
            }
            else
            {
                await socket.SendAsync(MsgType._AAA_PSInfo, msgA, pTimeoutS: null);
            }
        }
    }
}