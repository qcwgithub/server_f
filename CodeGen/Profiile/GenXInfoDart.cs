// 共用
public class GenXInfoDart
{
    public static string Gen(XInfoConfig xinfoConfig)
    {
        var f = new FileFormatter();
        f.SetTabWidth2();

        f.AddTab(2);

        GenFields(f, xinfoConfig);
        f.Push("\n");

        GenConstructor(f, xinfoConfig);
        f.Push("\n");

        GenToMsgPack(f, xinfoConfig);
        f.Push("\n");

        GenFromMsgPack(f, xinfoConfig);
        f.Push("\n");

        string str = f.GetString();
        return str;
    }

    public static void GenFields(FileFormatter f, XInfoConfig xinfoConfig)
    {
        // char alpha = (char)0;
        for (int i = 0; i < xinfoConfig.fields.Count; i++)
        {
            var fieldConfig = xinfoConfig.fields[i];

            f.PushTab().Push(fieldConfig.typeInfo.nameDart).Push(" ").Push(fieldConfig.name).Push(";");

            if (!string.IsNullOrEmpty(fieldConfig.comment))
            {
                f.Push(" // ", fieldConfig.comment);
            }

            f.Push("\n");
        }
    }

    public static void GenConstructor(FileFormatter f, XInfoConfig xinfoConfig)
    {
        f.TabPushF("{0}({{\n", xinfoConfig.name);
        f.AddTab(1);

        // char alpha = (char)0;
        for (int i = 0; i < xinfoConfig.fields.Count; i++)
        {
            var fieldConfig = xinfoConfig.fields[i];
            f.TabPushF("required this.{0},\n", fieldConfig.name);
        }

        f.AddTab(-1);
        f.TabPush("});\n");
    }
    public static void GenToMsgPack(FileFormatter f, XInfoConfig xinfoConfig)
    {
        f.PushTab().Push("List toMsgPack() {\n");
        f.AddTab(1);
        {
            f.TabPush("return [\n");
            f.AddTab(1);
            for (int i = 0; i < xinfoConfig.fields.Count; i++)
            {
                var config = xinfoConfig.fields[i];
                f.TabPush($"{config.name}, // [{i}]\n");
            }
            f.AddTab(-1);
            f.TabPush("];\n");
        }
        f.AddTab(-1);
        f.PushTab().Push("}\n");
    }
    public static void GenFromMsgPack(FileFormatter f, XInfoConfig xinfoConfig)
    {
        f.TabPushF("factory {0}.fromMsgPack(List list) {{\n", xinfoConfig.name);
        f.AddTab(1);
        {
            f.TabPushF("return {0}(\n", xinfoConfig.name);
            f.AddTab(1);
            for (int i = 0; i < xinfoConfig.fields.Count; i++)
            {
                var config = xinfoConfig.fields[i];
                f.TabPushF("{0}: ", config.name);
                f.PushDartFromMsgPack(config.typeInfo, $"list[{i}]");
            }
            f.AddTab(-1);
            f.PushTab().Push(");\n");
        }
        f.AddTab(-1);
        f.PushTab().Push("}\n");
    }
}