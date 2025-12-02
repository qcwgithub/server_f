using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;


namespace Script
{
    public class Command_Start : OnStart<CommandService>
    {
        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            // this.service.SetState(ServiceState.Starting);

            // base.StartKeepConnections();

            // base.StartSaveLocToRedis();

            // this.service.data.ListenForServer_Tcp();
            // this.service.data.ListenForClient_Tcp();      
            ECode e = await this.service.InitServiceConfigsUntilSuccess();

            MyResponse r = null;
            try
            {
                r = await this.DoHandle();
            }
            catch (System.Exception ex)
            {
                r = new MyResponse(ECode.Exception, null);
                this.service.logger.ErrorFormat(ex.Message);
            }
            finally
            {
                if (r.err != ECode.MonitorRunLoop)
                {
                    await this.service.data.CloseAllConnections();
                }

            }
            if (r.err != ECode.MonitorRunLoop)
            {
                this.service.logger.Info("exit after 1 second...");
                await Task.Delay(1000);
                Environment.Exit(r.err == ECode.Success ? 0 : 1);
            }

            return r;
        }

        string GetArg_Action()
        {
            var args = this.server.data.arguments;
            string action;
            if (!args.TryGetValue("action", out action))
            {
                throw new Exception("missing action");
            }
            return action;
        }

        public List<ServiceTypeAndId> GetArg_Targets()
        {
            var args = this.server.data.arguments;
            var targets = new List<ServiceTypeAndId>();
            string s;
            if (!args.TryGetValue("targets", out s))
            {
                throw new Exception("missing targets");
            }

            if (s == "all")
            {
                targets = this.service.data.thisServerServiceConfigs
                    .Select(sc => sc.Tai())
                    .Where(tai => !tai.serviceType.IsCommand())
                    .ToList();
            }
            else
            {
                targets.AddRange(s.Split(',').Select(raw => ServiceTypeAndId.FromString(raw)));

                for (int i = 0; i < targets.Count; i++)
                {
                    int serviceId = targets[i].serviceId;
                    for (int j = i + 1; j < targets.Count; j++)
                    {
                        if (targets[j].serviceId == serviceId)
                        {
                            throw new Exception("duplicate serviceId: " + serviceId);
                        }
                    }
                }
            }
            return targets;
        }

        long GetArg_UserId()
        {
            var args = this.server.data.arguments;
            string userIdString;
            long userId;
            if (!args.TryGetValue("userId", out userIdString) || !long.TryParse(userIdString, out userId))
            {
                throw new Exception("missing userId");
            }
            return userId;
        }

        string GetArg_String(string key, bool mustExist = true)
        {
            var args = this.server.data.arguments;
            string s;
            if (!args.TryGetValue(key, out s) && mustExist)
            {
                throw new Exception("missing " + key);
            }
            return s;
        }

        int GetArg_Int(string key, bool mustExist = true)
        {
            var args = this.server.data.arguments;
            string s;
            if (!args.TryGetValue(key, out s) && mustExist)
            {
                throw new Exception("missing " + key);
            }
            return s == null ? 0 : int.Parse(s);
        }

        bool GetArg_Int(string key, out int result)
        {
            var args = this.server.data.arguments;
            string s;
            if (args.TryGetValue(key, out s) && int.TryParse(s, out result))
            {
                return true;
            }
            result = 0;
            return false;
        }

        async Task<MyResponse> DoHandle()
        {
            string action = this.GetArg_Action();
            switch (action)
            {
                case "printUserUSId":
                    {
                        long userId = this.GetArg_UserId();
                        int psId = await this.server.playerPSRedis.GetPSId(userId);
                        this.service.logger.InfoFormat("userId: {0} psId: {1}", userId, psId);

                        return ECode.Success;
                    }

                case "taskQueueLengthes":
                    {
                        (long, long)[] lengthes = await this.server.persistence_taskQueueRedis.GetLengthInfo(PersistenceTaskQueueRedis.QUEUES);
                        for (int i = 0; i < PersistenceTaskQueueRedis.QUEUES.Length; i++)
                        {
                            this.service.logger.InfoFormat("taskQueue {0} list {1} zset {2}", PersistenceTaskQueueRedis.QUEUES[i], lengthes[i].Item1, lengthes[i].Item2);
                        }
                        return ECode.Success;
                    }

                default:
                    {
                        if (action.StartsWith("playerAction."))
                        {
                            return await this.PlayerAction(action.Substring("playerAction.".Length));
                        }
                        else
                        {
                            return await this.ServerAction(action);
                        }
                    }
            }
        }

        async Task<MyResponse> ServerAction(string action)
        {
            List<ServiceTypeAndId> targets = this.GetArg_Targets();
            if (targets.Count == 0)
            {
                this.service.logger.Info("targets.Count == 0");
                return ECode.Success;
            }

            foreach (var tai in targets)
            {
                if (null == this.service.data.current_resGetServiceConfigs.FindServiceConfig(tai.serviceType, tai.serviceId))
                {
                    this.service.logger.ErrorFormat("serviceId({0}) location not exist in redis, will do nothing!", tai.serviceId);
                    return ECode.Success;
                }
            }

            if (action == "shutdown")
            {
                // sort targets
                var order = this.server.data.shutdownServiceOrder;
                targets.Sort((a, b) =>
                {
                    if (a.serviceType == b.serviceType)
                    {
                        return 0;
                    }
                    return order.IndexOf(a.serviceType) - order.IndexOf(b.serviceType);
                });

                this.service.logger.InfoFormat("sort targets {0}", JsonUtils.stringify(targets.Select(x => x.ToString()).ToArray()));
            }

            // this.service.logger.Info("Start work");

            foreach (var tai in targets)
            {
                this.service.logger.Info("perform action to " + tai.ToString() + "...");

                int serviceId = tai.serviceId;
                ServiceConfig sc = this.service.data.current_resGetServiceConfigs.FindServiceConfig(tai.serviceType, tai.serviceId);
                ProtocolClientData socket = this.service.GetOrConnectSocket(sc.serviceType, sc.serviceId, sc.inIp, sc.inPort);

                while (socket.IsConnecting())
                {
                    await Task.Delay(1);
                }
                if (!socket.IsConnected())
                {
                    throw new System.Exception("error connect to " + tai.ToString());
                }
                // this.service.logger.Info("perform action to " + typeAndId.ToString() + "...");

                MyResponse r = null;
                switch (action)
                {
                    // 关服
                    case "shutdown":
                        {
                            r = await this.service.connectToSelf.SendToSelfAsync(MsgType._Command_PerformShutdown,
                                   new MsgCommon().SetLong("serviceId", serviceId).SetLong("force", this.GetArg_Int("force", false)));
                        }
                        break;

                    // 打印积压了多少消息
                    case "printPendingMsgList":
                        {
                            r = await this.service.connectToSelf.SendToSelfAsync(MsgType._Command_PerformGetPendingMsgList,
                                   new MsgCommon().SetLong("serviceId", serviceId));
                        }
                        break;

                    // 打印服务器 Script.dll 版本
                    case "showScriptVersion":
                        {
                            r = await this.service.connectToSelf.SendToSelfAsync(MsgType._Command_PerformShowScriptVersion,
                                   new MsgCommon().SetLong("serviceId", serviceId));
                        }
                        break;

                    // 重新加载 Script.dll
                    case "reloadScript":
                        {
                            string zip = this.GetArg_String("zip", false);

                            r = await this.service.connectToSelf.SendToSelfAsync(MsgType._Command_PerformReloadScript,
                                new MsgCommon().SetLong("serviceId", serviceId).SetString("zip", zip));
                        }
                        break;

                    // 重新加载配置表
                    case "reloadConfigs":
                        {
                            var msgReload = new MsgReloadConfigs();
                            string files = this.GetArg_String("files", false);
                            if (files == null || files.Length == 0)
                            {
                                r = ECode.Error;
                                break;
                            }
                            if (files == "all")
                            {
                                msgReload.all = true;
                            }
                            else
                            {
                                string[] array = files.Split(',');
                                msgReload.files = new List<string>();
                                msgReload.files.AddRange(array);
                            }
                            r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._ReloadConfigs, msgReload);
                        }
                        break;

                    case "getReloadConfigOptions":
                        {
                            var msgGet = new MsgGetReloadConfigOptions();
                            r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._GetReloadConfigOptions, msgGet);

                            if (r.err == ECode.Success)
                            {
                                var resGet = r.CastRes<ResGetReloadConfigOptions>();
                                for (int i = 0; i < resGet.files.Count; i++)
                                {
                                    this.service.logger.InfoFormat("{0}) {1}", i + 1, resGet.files[i]);
                                }
                            }
                        }
                        break;

                    // 执行一次 GC
                    case "gc":
                        {
                            var msgGc = new MsgGC();
                            r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._GC, msgGc);
                        }
                        break;

                    //
                    case "psAction":
                        {
                            var msg = new MsgPSAction();

                            string s = this.GetArg_String(nameof(msg.allowNewPlayer), false);
                            if (!string.IsNullOrEmpty(s))
                            {
                                msg.allowNewPlayer = s == "true" || s == "1";
                            }

                            if (this.GetArg_Int(nameof(msg.playerDestroyTimeoutS), out int i))
                            {
                                msg.playerDestroyTimeoutS = i;
                            }

                            if (this.GetArg_Int(nameof(msg.playerSaveIntervalS), out i))
                            {
                                msg.playerSaveIntervalS = i;
                            }

                            s = this.GetArg_String(nameof(msg.printTALogComsumerBufferSize), false);
                            if (!string.IsNullOrEmpty(s))
                            {
                                msg.printTALogComsumerBufferSize = s == "true" || s == "1";
                            }

                            s = this.GetArg_String(nameof(msg.clearTALogConsumerBuffer), false);
                            if (!string.IsNullOrEmpty(s))
                            {
                                msg.clearTALogConsumerBuffer = s == "true" || s == "1";
                            }

                            r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._ServerAction, msg);
                        }
                        break;

                    case "setAllowClientMinPatchVersion":
                        {
                            string android = this.GetArg_String(Platform.android, false);
                            string ios = this.GetArg_String(Platform.ios, false);

                            r = await this.service.connectToSelf.SendToSelfAsync(MsgType._Command_SetAllowClientMinPatchVersion,
                                new MsgCommon().SetLong("serviceId", serviceId).SetString(Platform.android, android).SetString(Platform.ios, ios));
                        }
                        break;

                    case "setGmFlag":
                        {
                            int startId = 0;
                            int endId = 0;
                            if (!int.TryParse(this.GetArg_String("startId"), out startId))
                            {
                                throw new System.Exception("missing startId");
                            }
                            if (!int.TryParse(this.GetArg_String("endId"), out endId))
                            {
                                throw new System.Exception("missing endId");
                            }
                            r = await this.service.connectToSelf.SendToSelfAsync(MsgType._Command_PerformSetPlayerGmFlag,
                                new MsgCommon().SetLong("serviceId", serviceId).SetLong("startId", startId).SetLong("endId", endId));
                        }
                        break;

                    case "saveUserProfileToFile": // 即使不在线也行的方法
                        {
                            long playerId = this.GetArg_UserId();
                            if (playerId == 0)
                            {
                                this.service.logger.InfoFormat("playerId is 0");
                                return ECode.Success;
                            }

                            var msgX = new MsgLoadPlayerNewestInfos();
                            msgX.what = LoadPlayerNewestWhat.Profile;
                            msgX.playerIds = new List<long> { playerId };

                            r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._PlayerS_LoadPlayerNewestInfos, msgX);
                            if (r.err == ECode.Success)
                            {
                                var res = r.CastRes<ResLoadPlayerNewestInfos>();

                                if (res.newestInfos.Count == 0)
                                {
                                    this.service.logger.Info("user not exist");
                                }
                                else
                                {
                                    string json = JsonUtils.stringify(res.newestInfos[0].profile);
                                    int zoneId = longidext.DecodeServerId(playerId);
                                    long rid = playerId % longidext.N;
                                    string fileName = $"profile{zoneId}_{rid}.json";
                                    File.WriteAllText(fileName, json);

                                    this.service.logger.Info("save profile to file ok, file name: " + fileName);
                                }
                            }
                        }
                        break;

                    case "showUserCount":
                        {
                            var msg2 = new MsgGetUserCount();
                            r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._GetPlayerCount, msg2);

                            if (r.err == ECode.Success)
                            {
                                var res = r.CastRes<ResGetUserCount>();
                                foreach (var kv in res.dict)
                                {
                                    this.service.logger.Info($"{kv.Key} = {kv.Value}");
                                }
                            }
                        }
                        break;

                    default:
                        throw new Exception("unknown action: '" + action + "'");
                }

                if (r.err != ECode.Success)
                {
                    throw new Exception(r.err.ToString());
                }
            }

            return ECode.Success;
        }

        async Task<MyResponse> PlayerAction(string action)
        {
            long playerId = this.GetArg_UserId();
            if (playerId == 0)
            {
                this.service.logger.InfoFormat("playerId is 0");
                return ECode.Success;
            }

            int usId = await this.server.playerPSRedis.GetPSId(playerId);
            // this.service.logger.InfoFormat("playerId: {0} psId: {1}", playerId, psId);
            if (usId == 0)
            {
                this.service.logger.InfoFormat("playerId: {0} psId is null", playerId);
                return ECode.Success;
            }

            this.service.logger.InfoFormat("playerId: {0} psId: {1}", playerId, usId);

            ServiceConfig sc = this.service.data.current_resGetServiceConfigs.FindServiceConfig(ServiceType.User, usId);
            if (sc == null)
            {
                this.service.logger.ErrorFormat("serviceId({0}) location not exist in redis, will do nothing!", usId);
                return ECode.Success;
            }

            ProtocolClientData socket = this.service.GetOrConnectSocket(sc.serviceType, sc.serviceId, sc.inIp, sc.inPort);
            while (socket.IsConnecting())
            {
                await Task.Delay(1);
            }
            if (!socket.IsConnected())
            {
                throw new System.Exception("error connect to " + usId.ToString());
            }

            MyResponse r = null;
            switch (action)
            {
                case "kick":
                    {
                        r = await this.service.connectToSelf.SendToSelfAsync(MsgType._Command_PerformKick,
                            new MsgCommon().SetLong("serviceId", usId).SetLong("playerId", playerId));
                    }
                    break;
                case "saveProfileToFile":
                    {
                        r = await this.service.connectToSelf.SendToSelfAsync(MsgType._Command_PerformSaveProfileToFile,
                            new MsgCommon().SetLong("serviceId", usId).SetLong("playerId", playerId));
                    }
                    break;
                    
                case "playerGM":
                case "finishAllGuide":
                case "finishAllGuideAndOpenAllFunctions":

                case "silence":
                case "un_silence":
                case "fakeSilence":
                case "un_fakeSilence":
                    {
                        string msgGMStr = null;

                        switch (action)
                        {
                            case "playerGM":
                                msgGMStr = this.GetArg_String("msgGM");
                                break;

                            case "finishAllGuide":
                                {
                                    MsgGM msgGM = new MsgGM();
                                    msgGM.guideFlags = new List<int>();
                                    for (GuideFlag e = 0; e < GuideFlag.Count; e++)
                                    {
                                        msgGM.guideFlags.Add((int)e);
                                    }
                                    msgGMStr = JsonUtils.stringify(msgGM);
                                }
                                break;
                        }

                        r = await this.service.connectToSelf.SendToSelfAsync(MsgType._Command_PerformPlayerGM,
                            new MsgCommon().SetLong("serviceId", usId).SetLong("playerId", playerId).SetString("msgGM", msgGMStr));
                    }
                    break;

                default:
                    this.service.logger.Info("Unknown action: " + action);
                    r = new MyResponse(ECode.Error);
                    break;
            }

            if (r.err != ECode.Success)
            {
                this.service.logger.Info("ECode." + r.err);
            }
            return r;
        }
    }
}