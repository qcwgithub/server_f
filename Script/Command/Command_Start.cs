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
        public Command_Start(Server server, CommandService service) : base(server, service)
        {
        }


        public override async Task<ECode> Handle(ProtocolClientData socket, MsgStart msg, ResStart res)
        {
            // this.service.SetState(ServiceState.Starting);

            // base.StartKeepConnections();

            // base.StartSaveLocToRedis();

            // this.service.data.ListenForServer_Tcp();
            // this.service.data.ListenForClient_Tcp();      
            ECode e = await this.service.InitServiceConfigsUntilSuccess();
            try
            {
                e = await this.DoHandle();
            }
            catch (System.Exception ex)
            {
                e = ECode.Exception;
                this.service.logger.ErrorFormat(ex.Message);
            }
            finally
            {
                if (e != ECode.MonitorRunLoop)
                {
                    await this.service.data.CloseAllConnections();
                }

            }
            if (e != ECode.MonitorRunLoop)
            {
                this.service.logger.Info("exit after 1 second...");
                await Task.Delay(1000);
                Environment.Exit(e == ECode.Success ? 0 : 1);
            }

            return e;
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
            string? userIdString;
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

        async Task<ECode> DoHandle()
        {
            string action = this.GetArg_Action();
            switch (action)
            {
                // case "printUserUSId":
                //     {
                //         long userId = this.GetArg_UserId();
                //         int psId = await this.server.playerPSRedis.GetPSId(userId);
                //         this.service.logger.InfoFormat("userId: {0} psId: {1}", userId, psId);

                //         return ECode.Success;
                //     }

                // case "taskQueueLengthes":
                //     {
                //         (long, long)[] lengthes = await this.server.persistence_taskQueueRedis.GetLengthInfo(PersistenceTaskQueueRedis.QUEUES);
                //         for (int i = 0; i < PersistenceTaskQueueRedis.QUEUES.Length; i++)
                //         {
                //             this.service.logger.InfoFormat("taskQueue {0} list {1} zset {2}", PersistenceTaskQueueRedis.QUEUES[i], lengthes[i].Item1, lengthes[i].Item2);
                //         }
                //         return ECode.Success;
                //     }

                default:
                    {
                        if (action.StartsWith("playerAction."))
                        {
                            return await this.UserAction(action.Substring("playerAction.".Length));
                        }
                        else
                        {
                            return await this.ServerAction(action);
                        }
                    }
            }
        }

        async Task<ECode> ServerAction(string action)
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

                ECode e;
                switch (action)
                {
                    // 关服
                    case "shutdown":
                        {
                            var r = await this.service.connectToSelf.Request<MsgCommon, ResCommon>(MsgType._Command_PerformShutdown,
                                   new MsgCommon().SetLong("serviceId", serviceId).SetLong("force", this.GetArg_Int("force", false)));
                            e = r.e;
                        }
                        break;

                    // 打印积压了多少消息
                    case "printPendingMsgList":
                        {
                            var r = await this.service.connectToSelf.Request<MsgCommon, ResCommon>(MsgType._Command_PerformGetPendingMsgList,
                                   new MsgCommon().SetLong("serviceId", serviceId));
                            e = r.e;
                        }
                        break;

                    // 打印服务器 Script.dll 版本
                    case "showScriptVersion":
                        {
                            var r = await this.service.connectToSelf.Request<MsgCommon, ResCommon>(MsgType._Command_PerformShowScriptVersion,
                                   new MsgCommon().SetLong("serviceId", serviceId));
                            e = r.e;
                        }
                        break;

                    // 重新加载 Script.dll
                    case "reloadScript":
                        {
                            string zip = this.GetArg_String("zip", false);

                            var r = await this.service.connectToSelf.Request<MsgCommon, ResCommon>(MsgType._Command_PerformReloadScript,
                                        new MsgCommon().SetLong("serviceId", serviceId).SetString("zip", zip));
                            e = r.e;
                        }
                        break;

                    // 重新加载配置表
                    case "reloadConfigs":
                        {
                            var msgReload = new MsgReloadConfigs();
                            string files = this.GetArg_String("files", false);
                            if (files == null || files.Length == 0)
                            {
                                e = ECode.Error;
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
                            var r = await this.service.connectToSameServerType.RequestToService<MsgReloadConfigs, ResReloadConfigs>(serviceId, MsgType._ReloadConfigs, msgReload);
                            e = r.e;
                        }
                        break;

                    case "getReloadConfigOptions":
                        {
                            var msgGet = new MsgGetReloadConfigOptions();
                            ResGetReloadConfigOptions resGet;
                            var r = await this.service.connectToSameServerType.RequestToService<MsgGetReloadConfigOptions, ResGetReloadConfigOptions>(serviceId, MsgType._GetReloadConfigOptions, msgGet);
                            e = r.e;

                            if (e == ECode.Success)
                            {
                                for (int i = 0; i < r.res.files.Count; i++)
                                {
                                    this.service.logger.InfoFormat("{0}) {1}", i + 1, r.res.files[i]);
                                }
                            }
                        }
                        break;

                    // 执行一次 GC
                    case "gc":
                        {
                            var msgGc = new MsgGC();
                            var r = await this.service.connectToSameServerType.RequestToService<MsgGC, ResGC>(serviceId, MsgType._GC, msgGc);
                            e = r.e;
                        }
                        break;

                    //
                    case "psAction":
                        {
                            var msg = new MsgPSAction();

                            string s = this.GetArg_String(nameof(msg.allowNewUser), false);
                            if (!string.IsNullOrEmpty(s))
                            {
                                msg.allowNewUser = s == "true" || s == "1";
                            }

                            if (this.GetArg_Int(nameof(msg.destroyTimeoutS), out int i))
                            {
                                msg.destroyTimeoutS = i;
                            }

                            if (this.GetArg_Int(nameof(msg.saveIntervalS), out i))
                            {
                                msg.saveIntervalS = i;
                            }

                            var r = await this.service.connectToSameServerType.RequestToService<MsgPSAction, ResPSAction>(serviceId, MsgType._ServerAction, msg);
                            e = r.e;
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
                            var r = await this.service.connectToSelf.Request<MsgCommon, ResCommon>(MsgType._Command_PerformSetPlayerGmFlag,
                                new MsgCommon().SetLong("serviceId", serviceId).SetLong("startId", startId).SetLong("endId", endId));
                            e = r.e;
                        }
                        break;

                    // case "saveUserProfileToFile": // 即使不在线也行的方法
                    //     {
                    //         long playerId = this.GetArg_UserId();
                    //         if (playerId == 0)
                    //         {
                    //             this.service.logger.InfoFormat("playerId is 0");
                    //             return ECode.Success;
                    //         }

                    //         var msgX = new MsgLoadPlayerNewestInfos();
                    //         msgX.what = LoadUserNewestWhat.Profile;
                    //         msgX.playerIds = new List<long> { playerId };

                    //         r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._PlayerS_LoadPlayerNewestInfos, msgX);
                    //         if (r.err == ECode.Success)
                    //         {
                    //             var res = r.CastRes<ResLoadPlayerNewestInfos>();

                    //             if (res.newestInfos.Count == 0)
                    //             {
                    //                 this.service.logger.Info("user not exist");
                    //             }
                    //             else
                    //             {
                    //                 string json = JsonUtils.stringify(res.newestInfos[0].profile);
                    //                 int zoneId = longidext.DecodeServerId(playerId);
                    //                 long rid = playerId % longidext.N;
                    //                 string fileName = $"profile{zoneId}_{rid}.json";
                    //                 File.WriteAllText(fileName, json);

                    //                 this.service.logger.Info("save profile to file ok, file name: " + fileName);
                    //             }
                    //         }
                    //     }
                    //     break;

                    case "showUserCount":
                        {
                            var msg2 = new MsgGetUserCount();
                            var r = await this.service.connectToSameServerType.RequestToService<MsgGetUserCount, ResGetUserCount>(serviceId, MsgType._GetPlayerCount, msg2);
                            e = r.e;

                            if (e == ECode.Success)
                            {
                                foreach (var kv in r.res.dict)
                                {
                                    this.service.logger.Info($"{kv.Key} = {kv.Value}");
                                }
                            }
                        }
                        break;

                    default:
                        throw new Exception("unknown action: '" + action + "'");
                }

                if (e != ECode.Success)
                {
                    throw new Exception(e.ToString());
                }
            }

            return ECode.Success;
        }

        async Task<ECode> UserAction(string action)
        {
            long userId = this.GetArg_UserId();
            if (userId == 0)
            {
                this.service.logger.InfoFormat("userId is 0");
                return ECode.Success;
            }

            int usId = 0;//await this.server.playerPSRedis.GetPSId(userId);
            // this.service.logger.InfoFormat("playerId: {0} psId: {1}", playerId, psId);
            if (usId == 0)
            {
                this.service.logger.InfoFormat("playerId: {0} psId is null", userId);
                return ECode.Success;
            }

            this.service.logger.InfoFormat("playerId: {0} psId: {1}", userId, usId);

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

            ECode e;
            switch (action)
            {
                case "kick":
                    {
                        var r = await this.service.connectToSelf.Request<MsgCommon, ResCommon>(MsgType._Command_PerformKick,
                            new MsgCommon().SetLong("serviceId", usId).SetLong("playerId", userId));
                        e = r.e;
                    }
                    break;
                case "saveProfileToFile":
                    {
                        var r = await this.service.connectToSelf.Request<MsgCommon, ResCommon>(MsgType._Command_PerformSaveProfileToFile,
                            new MsgCommon().SetLong("serviceId", usId).SetLong("playerId", userId));
                        e = r.e;
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
                        }

                        var r = await this.service.connectToSelf.Request<MsgCommon, ResCommon>(MsgType._Command_PerformPlayerGM,
                            new MsgCommon().SetLong("serviceId", usId).SetLong("playerId", userId).SetString("msgGM", msgGMStr));
                        e = r.e;
                    }
                    break;

                default:
                    this.service.logger.Info("Unknown action: " + action);
                    e = ECode.Error;
                    break;
            }

            if (e != ECode.Success)
            {
                this.service.logger.Info("ECode." + e);
            }
            return e;
        }
    }
}