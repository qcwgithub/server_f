using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class Create_XXXRedis
{
    public static void Create(ServerDataConfig config)
    {
        var ff = new FileFormatter();
        ff.TabPush("using System;\n");
        ff.TabPush("using System.Threading.Tasks;\n");
        ff.TabPush("using StackExchange.Redis;\n");
        ff.TabPush("using System.Collections.Generic;\n");
        ff.TabPush("using Data;\n");

        ff.Push("\n");
        ff.TabPush("namespace Script\n");
        ff.BlockStart();
        {
            ff.TabPush("//// AUTO CREATED ////\n");
            ff.TabPushF("public sealed class {0}Redis : IEntryScript<ScriptEntry>\n", config.xinfoType);
            ff.BlockStart();
            {
                ff.TabPushF("public ScriptEntry scriptEntry {{ get; private set; }}\n");
                ff.TabPushF("public {0}Redis(ScriptEntry scriptEntry)\n", config.xinfoType);
                ff.BlockStart();
                {
                    ff.TabPushF("this.scriptEntry = scriptEntry;\n");
                }
                ff.BlockEnd();

                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("public IDatabase GetDb()\n");
                ff.BlockStart();
                {
                    ff.TabPushF("return this.scriptEntry.dataEntry.redis.GetDatabase(this.scriptEntry.dataEntry.redisConfig.{0});\n", config.redisDb);
                }
                ff.BlockEnd();

                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("public string Key({0})\n", config.keyParamToString(true, true, string.Empty, false, false, false));
                ff.BlockStart();
                {
                    ff.TabPushF("return {0}({1});\n", config.keyFunc, config.keyParamToString(false, true, string.Empty, false, false, false));
                }
                ff.BlockEnd();

                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("public async Task<{0}> Get({1})\n", config.xinfoType, config.keyParamToString(true, true, string.Empty, true, true, true));
                ff.BlockStart();
                {
                    ff.TabPushF("return await RedisUtils.GetAsJson<{0}>(this.GetDb(), this.Key({1}));\n", config.xinfoType, config.keyParamToString(false, true, string.Empty, false, false, false));
                }
                ff.BlockEnd();

                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("public async Task Save({0} info, TimeSpan? expiry)\n", config.xinfoType);
                ff.BlockStart();
                {
                    ff.TabPushF("await RedisUtils.SaveAsJson(this.GetDb(), this.Key({0}), info, expiry);\n", string.Join(", ", config.keyParam.Select(_ => "info." + _.name)));
                }
                ff.BlockEnd();
            }
            ff.BlockEnd();
        }
        ff.BlockEnd();

        string dir = "Script/Common/Redis/";
        Directory.CreateDirectory(dir);
        string path = dir + config.xinfoType + "Redis.cs";
        File.WriteAllText(path, ff.GetString());
    }
}