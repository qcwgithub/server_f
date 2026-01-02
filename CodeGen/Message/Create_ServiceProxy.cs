public class Create_ServiceProxy
{
    public static void Create(List<MessageConfig> configs)
    {
        var proxyDict = new Dictionary<string, FileFormatter>();

        for (int i = 0; i < configs.Count; i++)
        {
            var config = configs[i];
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
                serviceType = string.Empty; // !
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

            // Proxy
            if (config.external)
            {
                FileFormatter f;
                if (!proxyDict.ContainsKey(serviceType))
                {
                    f = new FileFormatter();
                    f.AddTab(2);
                    proxyDict.Add(serviceType, f);
                }
                else
                {
                    f = proxyDict[serviceType];
                }

                f.TabPush($"public async Task<MyResponse> {methodName}(");
                if (config.arg_serviceId)
                {
                    f.Push($"int serviceId, ");
                }
                f.Push($"{config.msg} msg)\n");

                f.BlockStart();
                f.TabPush($"return await this.Request(");
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
            }
        }

        foreach (var kv in proxyDict)
        {
            string serviceType = kv.Key;

            string path;
            if (string.IsNullOrEmpty(serviceType))
            {
                path = $"Script/Common/ServiceProxy.cs";
            }
            else
            {
                path = $"Script/{serviceType}/{serviceType}ServiceProxy.cs";
            }

            XInfoProgram.ReplaceFile(path, new Mark[]
            {
                new Mark { startMark = "#region auto", text = kv.Value.GetString() },
            });
        }
    }
}