public class MessageProgram
{
    public static void MainX()
    {
        Script.CsvHelper helper = Script.CsvUtils.Parse(CodeGen.Program.ReadAllText("CodeGen/MessageTypeConfig.csv"));
        var list = new List<MessageTypeConfig>();
        while (helper.ReadRow())
        {
            var c = new MessageTypeConfig();
            c.msgType = helper.ReadString("msgType");
            c.value = helper.ReadInt("value");
            c.msg = helper.ReadString("msg");
            c.res = helper.ReadString("res");
            c.queue = helper.ReadString("queue");
            c.external = helper.ReadString("external") == "1";
            c.arg_serviceId = helper.ReadString("arg_serviceId") == "1";

            //
            c.isServer = c.msgType[0] == '_';
            if (c.isServer)
            {
                int index2 = c.msgType.IndexOf('_', 1);
                if (index2 < 0)
                {
                    c.serviceType = CGServiceType.Base;
                    c.methodName = c.msgType.Substring(1);
                }
                else
                {
                    string key = c.msgType.Substring(1, index2 - 1);

                    switch (key)
                    {
                        case "Save":
                        case "Query":
                        case "Insert":
                        case "Search":
                            c.serviceType = CGServiceType.Db;
                            c.methodName = key + "_" + c.msgType.Substring(index2 + 1);
                            break;

                        default:
                            if (Enum.TryParse<CGServiceType>(key, false, out CGServiceType ct))
                            {
                                c.serviceType = ct;
                                c.methodName = c.msgType.Substring(index2 + 1);
                            }
                            else
                            {
                                c.serviceType = CGServiceType.Base;
                                c.methodName = c.msgType.Substring(1);
                            }
                            break;
                    }

                }

            }

            list.Add(c);
        }

        Create_MsgType.Create(list);
        Create_MessageTypeConfigData.Create(list);
        Create_ServiceProxy.Create(list);

        
    }
}