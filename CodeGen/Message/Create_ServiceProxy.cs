public class Create_ServiceProxy
{
    public static void Create(List<MessageTypeConfig> configs)
    {
        var proxyDict = new Dictionary<CGServiceType, FileFormatter>();

        for (int i = 0; i < configs.Count; i++)
        {
            var config = configs[i];
            if (!config.isServer)
            {
                continue;
            }

            // Proxy
            if (config.external)
            {
                FileFormatter f;
                if (!proxyDict.ContainsKey(config.serviceType))
                {
                    f = new FileFormatter();
                    f.AddTab(2);
                    proxyDict.Add(config.serviceType, f);
                }
                else
                {
                    f = proxyDict[config.serviceType];
                }

                f.TabPush($"public async Task<MyResponse> {config.methodName}(");
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
                    f.Push($"ServiceType.{config.serviceType}, ");
                }
                f.Push($"MsgType.{config.msgType}, msg);\n");
                f.BlockEnd();
            }
        }

        foreach (var kv in proxyDict)
        {
            CGServiceType serviceType = kv.Key;

            string path;
            if (serviceType == CGServiceType.Base)
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