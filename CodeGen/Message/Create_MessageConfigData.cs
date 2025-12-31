public class Create_MessageConfigData
{
    public static void Create(List<MessageConfig> configs)
    {
        var init = new FileFormatter();
        init.AddTab(3);

        var deserialize = new FileFormatter();
        deserialize.AddTab(4);

        var serialize = new FileFormatter();
        serialize.AddTab(4);

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

            deserialize.TabPushF("case MsgType.{0}:\n", config.msgType);
            deserialize.AddTab(1);
            if (!string.IsNullOrEmpty(config.msg))
            {
                deserialize.TabPushF("return MessagePackSerializer.Deserialize<{0}>(msg);\n", config.msg);
            }
            else
            {
                deserialize.TabPushF("throw new Exception(\"Missing config for MsgType.{0}\");\n", config.msgType);
            }
            deserialize.AddTab(-1);

            if (i < configs.Count - 1)
            {
                deserialize.Push("\n");
            }

            ////

            serialize.TabPushF("case MsgType.{0}:\n", config.msgType);
            serialize.AddTab(1);
            if (!string.IsNullOrEmpty(config.msg))
            {
                serialize.TabPushF("return MessagePackSerializer.Serialize<{0}>(({0})res);\n", config.msg);
            }
            else
            {
                serialize.TabPushF("throw new Exception(\"Missing config for MsgType.{0}\");\n", config.msgType);
            }
            serialize.AddTab(-1);

            if (i < configs.Count - 1)
            {
                serialize.Push("\n");
            }
        }

        XInfoProgram.ReplaceFile("Data/Common/MessageConfigData.InitDict.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = init.GetString() },
        });

        XInfoProgram.ReplaceFile("Data/Common/MessageConfigData.DeserializeMsg.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = deserialize.GetString() },
        });

        XInfoProgram.ReplaceFile("Data/Common/MessageConfigData.SerializeRes.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = serialize.GetString() },
        });
    }
}