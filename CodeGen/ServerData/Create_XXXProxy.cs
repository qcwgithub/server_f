using System;
using System.IO;
using System.Linq;
public class Create_XXXProxy
{
    public static void Create(ServerDataConfig config)
    {
        var ff = new FileFormatter();
        ff.TabPush("using System;\n");
        ff.TabPush("using System.Diagnostics;\n");
        ff.TabPush("using System.Threading.Tasks;\n");
        ff.TabPush("using StackExchange.Redis;\n");
        ff.TabPush("using System.Collections.Generic;\n");
        ff.TabPush("using Data;\n");

        config.PushUsingOriginal(ff);

        ff.Push("\n");
        ff.TabPush("namespace Script\n");
        ff.BlockStart();
        {
            // ServerDataConfig.Field[] keyParam = new ServerDataConfig.Field[2];

            // for (int j = 0; j < 2; j++)
            // {
            //     if (j < config.keyParam.Count)
            //     {
            //         keyParam[j] = config.keyParam[j];
            //     }
            //     else
            //     {
            //         keyParam[j] = new ServerDataConfig.Field
            //         {
            //             type = "int",
            //             name = "_"
            //         };
            //     }
            // }

            // string keyParam_types = string.Join(", ", keyParam.Select(_ => _.type));
            // string keyParam_c = string.Join(", ", config.keyParam.Select(_ => _.name));
            // string keyParam_f_full = string.Join(", ", config.keyParam.Select(_ => _.type + " " + _.name));

            if (config.copy != null && config.copy.index == 0)
            {
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("public interface I{0}Proxy\n", config.profileType);
                ff.BlockStart();
                {
                    ff.TabPushF("Task<{0}> OnlyForSave_GetFromRedis({1});\n", config.profileType, config.keyParamToString(true, true, string.Empty, false, false, false));
                    ff.TabPushF("Task<{0}> Get({1});\n", config.profileType, config.keyParamToString(true, true, string.Empty, false, false, false));
                    ff.TabPushF("Task Save({0} info);\n", config.profileType);
                }
                ff.BlockEnd();
                ff.Push("\n");
            }

            ff.TabPush("//// AUTO CREATED ////\n");
            ff.TabPushF("public {2} class {0}Proxy{3} : DataProxy<{0}, {1}>", config.profileType, config.keyParamToString(true, false, string.Empty, true, false, false),
                config.proxyIsPartial ? "partial" : "sealed", config.postfix);
            if (config.copy != null)
            {
                ff.Push($", I{config.profileType}Proxy");
            }
            ff.Push("\n");
            ff.BlockStart();
            {
                // // ff.TabPushF("{0}Redis infoRedis;\n", config.profileType);
                // ff.TabPush("//// AUTO CREATED ////\n");
                // ff.TabPushF("public {0}Proxy{1}(BaseServer baseServer) : base(baseServer)\n", config.profileType, config.postfix);
                // ff.BlockStart();
                // {
                //     // ff.TabPushF("this.scriptEntry = scriptEntry;\n");
                //     // ff.TabPushF("this.infoRedis = new {0}Redis(scriptEntry);\n", config.profileType);
                // }
                // ff.BlockEnd();

                ff.TabPushF("#region override\n");


                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                // ff.TabPushF("protected override IDatabase GetDb({0})\n", config.keyParamToString(true, true, string.Empty, true, true, true));
                ff.TabPushF("protected override IDatabase GetDb()\n");
                ff.BlockStart();
                {
                    // ff.TabPushF("return this.infoRedis.GetDb();\n");
                    ff.TabPushF("return this.server.baseServerData.redis_db;\n", config.redisDb);
                }
                ff.BlockEnd();

                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("protected override stDirtyElement DirtyElement({0})\n", config.keyParamToString(true, true, string.Empty, true, true, true));
                ff.BlockStart();
                {
                    ff.TabPushF("return stDirtyElement.Create_{0}{1}({2});\n", config.profileType, config.postfix, config.keyParamToString(false, true, string.Empty, false, false, false));
                }
                ff.BlockEnd();

                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("protected override RedisKey Key({0})\n", config.keyParamToString(true, true, string.Empty, true, true, true));
                ff.BlockStart();
                {
                    // ff.TabPushF("return this.infoRedis.Key({0});\n", config.keyParamToString(false, true, string.Empty, false, false, false));
                    ff.TabPushF("return {0}({1});\n", config.keyFunc, config.InsertRedisKeyParam(config.keyParamToString(false, true, string.Empty, false, false, false)));
                }
                ff.BlockEnd();


                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("protected override bool CanExpire()\n");
                ff.BlockStart();
                {
                    ff.TabPushF("return {0};\n", config.canExpire ? "true" : "false");
                }
                ff.BlockEnd();


                // ff.Push("\n");
                // ff.TabPush("//// AUTO CREATED ////\n");
                // ff.TabPushF("protected override async Task<{0}> GetFromRedis({1})\n", config.profileType, config.keyParamToString(true, true, string.Empty, true, true, true));
                // ff.BlockStart();
                // {
                // ff.TabPushF("{0} info = await this.infoRedis.Get({1});\n", config.profileType, config.keyParamToString(false, true, string.Empty, false, false, false));
                // ff.TabPushF("return info;\n");
                // switch (config.valueFormat)
                // {
                // case "json":
                // ff.TabPushF("return await RedisUtils.GetAsJson<{0}>(this.GetDb(), this.Key({1}));\n", config.profileType, config.keyParamToString(false, true, string.Empty, false, false, false));
                // break;
                // default:
                // throw new NotImplementedException();
                // }
                // }
                // ff.BlockEnd();

                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("public Task<{0}> OnlyForSave_GetFromRedis({1})\n", config.profileType, config.keyParamToString(true, true, string.Empty, false, false, false));
                ff.BlockStart();
                {
                    ff.TabPushF("return this.GetFromRedis({0});\n", config.keyParamToString(false, true, string.Empty, false, false, true));
                }
                ff.BlockEnd();


                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("protected override {0} CreatePlaceholder({1})\n", config.profileType, config.keyParamToString(true, true, string.Empty, true, true, true));
                var query = config.query[config.loadUseQueryIndex];
                ff.BlockStart();
                {
                    if (config.createPlaceholderWhenNull)
                    {
                        ff.TabPushF("var placeholder = new {0}();\n", config.profileType);
                        foreach (var p in query.methodParamExps)
                        {
                            ff.TabPushF("placeholder.{0} = {0};\n", p.Substring(p.LastIndexOf(' ') + 1));
                        }
                        ff.TabPushF("placeholder.SetIsPlaceholder();\n");
                        ff.TabPushF("return placeholder;\n");

                    }
                    else
                    {
                        ff.TabPushF("throw new NotImplementedException();\n");
                    }
                }
                ff.BlockEnd();


                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("protected override string GetLockKeyForLoadFromDBToRedis({0})\n", config.keyParamToString(true, true, string.Empty, true, true, true));
                ff.BlockStart();
                {
                    ff.TabPushF("return LockKey.LoadDataFromDBToRedis.{0}({1});\n", config.profileType, config.InsertRedisKeyParam(config.keyParamToString(false, true, string.Empty, false, false, false)));
                }
                ff.BlockEnd();


                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                string jjjj = config.keyParamToString(true, true, string.Empty, true, true, true);
                ff.TabPushF("protected override async Task<(ECode, {0})> LoadFromDB(IConnectToDBService connectToDBService{1}{2})\n",
                    config.profileType,
                    jjjj.Length > 0 ? ", " : "",
                    jjjj);
                ff.BlockStart();
                {

                    ff.TabPushF("var msgDb = new Msg{0}();\n", query.methodName);
                    // ff.TabPushF("msgDb.playerId = playerId;\n");
                    foreach (var p in query.methodParamExps)
                    {
                        ff.TabPushF("msgDb.{0} = {0};\n", p.Substring(p.LastIndexOf(' ') + 1));
                    }

                    ff.TabPushF("MyResponse r = await connectToDBService.SendAsync(MsgType._{0}{1}, msgDb);\n",
                        query.methodName,
                        config.postfix);
                    ff.TabPushF("if (r.err != ECode.Success)\n");
                    ff.BlockStart();
                    {
                        ff.TabPushF("return (r.err, null);\n");
                    }
                    ff.BlockEnd();
                    ff.Push("\n");
                    ff.TabPushF("var res = r.CastRes<Res{0}>().result;\n", query.methodName);

                    if (!config.createPlaceholderWhenNull)
                    {
                        ff.TabPushF("if (res == null)\n");
                        ff.BlockStart();
                        {
                            ff.TabPushF("res = new {0}().CreateDefaultForRedis({1});\n", config.profileType, config.keyParamToString(false, true, string.Empty, false, false, false));
                        }
                        ff.BlockEnd();
                    }

                    ff.TabPushF("return (ECode.Success, res);\n", query.methodName);
                }
                ff.BlockEnd();



                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("protected override int GetBelongTaskQueue({0})\n", config.keyParamToString(true, true, string.Empty, true, true, true));
                ff.BlockStart();
                {
                    string p = string.Empty;
                    switch (config.taskQueueHash)
                    {
                        case ServerDataConfig.TaskQueueHash.NoKey:
                            p = string.Empty;
                            break;

                        case ServerDataConfig.TaskQueueHash.FirstKey:
                            p = config.keyParam[0].name;
                            break;

                        case ServerDataConfig.TaskQueueHash.TwoKeys:
                            p = string.Format("{0}, {1}", config.keyParam[0].name, config.keyParam[1].name);
                            break;

                        case ServerDataConfig.TaskQueueHash.SecondKey:
                            p = config.keyParam[1].name;
                            break;

                        default:
                            throw new Exception("Not handled TaskQueueHash." + config.taskQueueHash);
                    }

                    ff.TabPushF("return PersistenceTaskQueueRedis.GetQueue({0}.ToTaskQueueHash({1}));\n", config.profileType, p);
                }
                ff.BlockEnd();
                // ff.Push("\n");
                // ff.TabPush("//// AUTO CREATED ////\n");
                // ff.TabPush("protected override bool SaveImmediately()\n");
                // ff.BlockStart();
                // {
                //     ff.TabPushF("return {0};\n", config.saveImmediately ? "true" : "false");
                // }
                // ff.BlockEnd();

                // ff.Push("\n");
                // ff.TabPush("//// AUTO CREATED ////\n");
                // ff.TabPushF("protected override async Task<ECode> SaveToDB({0} info)\n", config.profileType, config.keyParamToString(true, true, string.Empty, true, true, true));
                // ff.BlockStart();
                // {
                //     Create_PersistenceOnSave_XXX.SaveToDB2(config, ff);
                // }
                // ff.BlockEnd();

                // ff.Push("\n");
                // ff.TabPush("//// AUTO CREATED ////\n");
                // ff.TabPushF("protected override async Task SaveToRedis({0}, {1} info, TimeSpan? expiry)\n", config.keyParamToString(true, true, string.Empty, true, true, false), config.profileType);
                // ff.BlockStart();
                // {
                //     // ff.TabPushF("await this.infoRedis.Save(info, expiry);\n");
                //     switch (config.valueFormat)
                //     {
                //         case "json":
                //             ff.TabPushF("await RedisUtils.SaveAsJson(this.GetDb(), this.Key({0}), info, expiry);\n", string.Join(", ", config.keyParam.Select(_ => "info." + _.name)));
                //             break;
                //         default:
                //             throw new NotImplementedException();
                //     }
                // }
                // ff.BlockEnd();

                ff.TabPushF("#endregion override\n");

                ff.Push("\n");
                ff.TabPushF("/////////////////////////////////////////// PUBLIC ///////////////////////////////////////////\n");
                ff.TabPush("//// AUTO CREATED ////\n");

                string ssss = config.keyParamToString(true, true, string.Empty, false, false, false);
                ff.TabPushF("public async Task<{0}> Get(ConnectTo{1}Service connectTo{1}Service{2}{3})\n",
                    config.profileType,
                    config.dbFilesConfig.serviceType,
                    ssss.Length > 0 ? ", " : "",
                    ssss);
                ff.BlockStart();
                {
                    if (config.keyParam.Count > 0)
                    {
                        ff.TabPushF("if (");

                        int x = 0;
                        foreach (var field in config.keyParam)
                        {
                            if (x > 0)
                            {
                                ff.Push(" && ");
                            }

                            switch (field.type)
                            {
                                case "int":
                                case "long":
                                case "longid":
                                    ff.Push(string.Format("{0} == 0", field.name));
                                    break;
                                case "string":
                                    ff.Push(string.Format("string.IsNullOrEmpty({0})", field.name));
                                    break;
                                default:
                                    if (field.typeModifier == "enum")
                                    {
                                        ff.Push(string.Format("{0}.IsEnumValueValid()", field.name));
                                    }
                                    else
                                    {
                                        throw new NotImplementedException();
                                    }
                                    break;
                            }
                            x++;
                        }

                        ff.Push(")\n");
                        ff.BlockStart();
                        ff.TabPushF("MyDebug.Assert(false);\n");
                        ff.TabPushF("return null;\n");
                        ff.BlockEnd();
                    }

                    string kkkk = config.keyParamToString(false, true, string.Empty, false, false, true);
                    ff.TabPushF("var info = await base.InternalGet(connectTo{0}Service{1}{2});\n",
                        config.dbFilesConfig.serviceType,
                        kkkk.Length > 0 ? ", " : "",
                        kkkk);
                    if (config.keyParam.Count > 0)
                    {
                        ff.TabPush("if (info != null)\n");
                        ff.BlockStart();
                        {
                            foreach (var field in config.keyParam)
                            {
                                ff.TabPushF("MyDebug.Assert(info.{0} == {0});\n", field.name);
                            }
                        }
                        ff.BlockEnd();
                    }
                    ff.TabPush("if (info != null)\n");
                    ff.BlockStart();
                    {
                        ff.TabPushF("info.Ensure();\n");
                    }
                    ff.BlockEnd();
                    ff.TabPush("return info;\n");
                }
                ff.BlockEnd();

                ff.Push("\n");

                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("public async Task Save({0} info)\n", config.profileType);
                ff.BlockStart();
                {
                    // MyDebug.Assert(info.unionId > 0);
                    ff.TabPushF("await base.SaveToRedis_Persist_IncreaseDirty({0}, info);\n", config.keyParamToString(false, true, "info.", false, false, true));
                }
                ff.BlockEnd();

                // if (config.keyParam.Count == 1)
                // {
                //     ff.Push("\n");
                //     ff.TabPush("//// AUTO CREATED ////\n");
                //     ff.TabPushF("public async Task<List<{0}>> GetMany(List<{1}> idList)\n", config.profileType, config.keyParam[0].type);
                //     ff.BlockStart();
                //     {
                //         ff.TabPushF("Task<{0}>[] tasks = new Task<{0}>[idList.Count];\n", config.profileType);
                //         ff.TabPushF("for (int i = 0; i < idList.Count; i++)\n");
                //         ff.BlockStart();
                //         {
                //             ff.TabPushF("tasks[i] = base.InternalGet(idList[i], 0);\n");
                //         }
                //         ff.BlockEnd();
                //         ff.TabPushF("await Task.WhenAll(tasks);\n");

                //         ff.Push("\n");
                //         ff.TabPushF("var retList = new List<{0}>();\n", config.profileType);
                //         ff.TabPushF("foreach (var task in tasks)\n");
                //         ff.BlockStart();
                //         {
                //             ff.TabPushF("retList.Add(task.Result);\n");
                //         }
                //         ff.BlockEnd();
                //         ff.TabPushF("return retList;\n");
                //     }
                //     ff.BlockEnd();
                // }
            }
            ff.BlockEnd();
        }
        ff.BlockEnd();

        string dir = "Script/Common/DataProxy/";
        Directory.CreateDirectory(dir);
        string path = dir + config.profileType + "Proxy" + config.postfix + ".cs";
        File.WriteAllText(path, ff.GetString());
    }
}