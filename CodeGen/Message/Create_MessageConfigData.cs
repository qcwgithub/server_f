public class Create_MessageConfigData
{
    public static void Create(List<MessageTypeConfig> configs)
    {
        var init = new FileFormatter();
        init.AddTab(3);

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

            init.TabPushF("dict[MsgType.{0}] = new stMessageConfig\n", config.msgType);

            init.BlockStart();
            init.TabPushF("queue = MessageQueue.{0},\n", string.IsNullOrEmpty(config.queue) ? "None" : config.queue);
            init.AddTab(-1);
            init.PushTab().Push("};\n");

            if (i < configs.Count - 1)
            {
                init.Push("\n");
            }

            ////

            serializeMsg.TabPushF("case MsgType.{0}:\n", config.msgType);
            serializeMsg.AddTab(1);
            if (!string.IsNullOrEmpty(config.msg))
            {
                serializeMsg.TabPushF("return MessagePackSerializer.Serialize(({0})msg);\n", config.msg);
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
                deserializeMsg.TabPushF("return MessagePackSerializer.Deserialize<{0}>(msgBytes);\n", config.msg);
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
                serializeRes.TabPushF("return MessagePackSerializer.Serialize(({0})res);\n", config.res);
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
                deserializeRes.TabPushF("return MessagePackSerializer.Deserialize<{0}>(resBytes);\n", config.res);
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

        XInfoProgram.ReplaceFile("Data/Common/MessageConfigData.InitDict.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = init.GetString() },
        });

        XInfoProgram.ReplaceFile("Data/Common/MessageConfigData.SerializeMsg.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = serializeMsg.GetString() },
        });

        XInfoProgram.ReplaceFile("Data/Common/MessageConfigData.DeserializeMsg.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = deserializeMsg.GetString() },
        });

        XInfoProgram.ReplaceFile("Data/Common/MessageConfigData.SerializeRes.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = serializeRes.GetString() },
        });

        XInfoProgram.ReplaceFile("Data/Common/MessageConfigData.DeserializeRes.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = deserializeRes.GetString() },
        });
    }
}