public class Create_ServiceProxy
{
    public static void Create(List<MessageConfig> configs)
    {
        var fdict = new Dictionary<string, FileFormatter>();

        for (int i = 0; i < configs.Count; i++)
        {
            var config = configs[i];
            if (!config.external)
            {
                continue;
            }
            if (config.msgType[0] != '_')
            {
                continue;
            }

            int index2 = config.msgType.IndexOf('_', 1);
            if (index2 < 0)
            {
                continue;
            }
            string methodName;

            string key = config.msgType.Substring(1, index2 - 1);
            string serviceType;
            if (key == "Service")
            {
                serviceType = ""; // !
                methodName = config.msgType.Substring(index2 + 1);
            }
            else
            {
                switch (key)
                {
                    case "Save":
                    case "Query":
                    case "Insert":
                        serviceType = "Db";
                        methodName = key + "_" + config.msgType.Substring(index2 + 1);
                        break;

                    default:
                        serviceType = key;
                        methodName = config.msgType.Substring(index2 + 1);
                        break;
                }
            }

            FileFormatter f;
            if (!fdict.ContainsKey(serviceType))
            {
                f = new FileFormatter();
                f.AddTab(2);
                fdict.Add(serviceType, f);
            }
            else
            {
                f = fdict[serviceType];
            }

            f.TabPush($"public async Task<MyResponse<{config.res}>> {methodName}(");
            if (config.arg_serviceId)
            {
                f.Push($"int serviceId, ");
            }
            f.Push($"{config.msg} msg)\n");

            f.BlockStart();
            f.TabPush($"return await this.Request<{config.msg}, {config.res}>(");
            if (config.arg_serviceId)
            {
                f.Push($"serviceId, ");
            }
            else
            {
                f.Push($"ServiceType.{serviceType}, ");
            }
            f.Push($"MsgType.{config.msgType}, msg);\n");
            f.BlockEnd();

            if (i < configs.Count - 1)
            {
                f.Push("\n");
            }
        }

        foreach (var kv in fdict)
        {
            XInfoProgram.ReplaceFile($"Script/Common/ServiceProxy/{kv.Key}ServiceProxy.cs", new Mark[]
            {
                new Mark { startMark = "#region auto", text = kv.Value.GetString() },
            });
        }
    }
}