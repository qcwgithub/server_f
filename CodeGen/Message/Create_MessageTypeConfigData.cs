public class Create_MessageTypeConfigData
{
    public static void Create(List<MessageTypeConfig> configs)
    {
        var serializeMsg = new FileFormatter();
        serializeMsg.AddTab(4);

        var deserializeMsg = new FileFormatter();
        deserializeMsg.AddTab(4);

        var serializeRes = new FileFormatter();
        serializeRes.AddTab(4);
        
        var deserializeRes = new FileFormatter();
        deserializeRes.AddTab(4);

        for (int i = 0; i < configs.Count; i++)
        {
            var config = configs[i];

            ////

            serializeMsg.TabPushF("case MsgType.{0}:\n", config.msgType);
            serializeMsg.AddTab(1);
            if (!string.IsNullOrEmpty(config.msg))
            {
                serializeMsg.TabPushF("msgBytes = MessagePackSerializer.Serialize(({0})msg);\n", config.msg);
                serializeMsg.TabPushF("break;\n");
            }
            else
            {
                serializeMsg.TabPushF("throw new Exception(\"Missing config for MsgType.{0}\");\n", config.msgType);
            }
            serializeMsg.AddTab(-1);

            if (i < configs.Count - 1)
            {
                serializeMsg.Push("\n");
            }

            ////

            deserializeMsg.TabPushF("case MsgType.{0}:\n", config.msgType);
            deserializeMsg.AddTab(1);
            if (!string.IsNullOrEmpty(config.msg))
            {
                deserializeMsg.TabPushF("ob = MessagePackSerializer.Deserialize<{0}>(msgBytes);\n", config.msg);
                deserializeMsg.TabPushF("break;\n");
            }
            else
            {
                deserializeMsg.TabPushF("throw new Exception(\"Missing config for MsgType.{0}\");\n", config.msgType);
            }
            deserializeMsg.AddTab(-1);

            if (i < configs.Count - 1)
            {
                deserializeMsg.Push("\n");
            }

            ////

            serializeRes.TabPushF("case MsgType.{0}:\n", config.msgType);
            serializeRes.AddTab(1);
            if (!string.IsNullOrEmpty(config.res))
            {
                serializeRes.TabPushF("msgBytes = MessagePackSerializer.Serialize(({0})res);\n", config.res);
                serializeRes.TabPushF("break;\n");
            }
            else
            {
                serializeRes.TabPushF("throw new Exception(\"Missing config for MsgType.{0}\");\n", config.msgType);
            }
            serializeRes.AddTab(-1);

            if (i < configs.Count - 1)
            {
                serializeRes.Push("\n");
            }

            ////

            deserializeRes.TabPushF("case MsgType.{0}:\n", config.msgType);
            deserializeRes.AddTab(1);
            if (!string.IsNullOrEmpty(config.res))
            {
                deserializeRes.TabPushF("ob = MessagePackSerializer.Deserialize<{0}>(resBytes);\n", config.res);
                deserializeRes.TabPushF("break;\n");
            }
            else
            {
                deserializeRes.TabPushF("throw new Exception(\"Missing config for MsgType.{0}\");\n", config.msgType);
            }
            deserializeRes.AddTab(-1);

            if (i < configs.Count - 1)
            {
                deserializeRes.Push("\n");
            }
        }

        XInfoProgram.ReplaceFile("Data/Common/MessageTypeConfigData.SerializeMsg.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = serializeMsg.GetString() },
        });

        XInfoProgram.ReplaceFile("Data/Common/MessageTypeConfigData.DeserializeMsg.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = deserializeMsg.GetString() },
        });

        XInfoProgram.ReplaceFile("Data/Common/MessageTypeConfigData.SerializeRes.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = serializeRes.GetString() },
        });

        XInfoProgram.ReplaceFile("Data/Common/MessageTypeConfigData.DeserializeRes.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = deserializeRes.GetString() },
        });
    }
}