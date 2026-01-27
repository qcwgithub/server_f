// 共用
public class GenXInfoDart
{
    public static void Gen(XInfoConfig xinfoConfig)
    {
        var f = new FileFormatter();
        f.SetTabWidth2();

        GenImports(f, xinfoConfig);

        f.TabPush($"class {xinfoConfig.name} {{\n");

        f.AddTab(2);

        GenFields(f, xinfoConfig);
        f.Push("\n");

        GenConstructor(f, xinfoConfig);
        f.Push("\n");

        GenToMsgPack(f, xinfoConfig);
        f.Push("\n");

        GenFromMsgPack(f, xinfoConfig);
        f.AddTab(-2);

        f.TabPush("}");

        File.WriteAllText("../client_f/lib/gen/" + xinfoConfig.fileNameDart + ".dart",
            f.GetString());
    }

    static void FindClass(FieldTypeInfo typeInfo, List<string> classNames)
    {
        if (typeInfo.type == FieldType.class_||
            typeInfo.type == FieldType.enum_)
        {
            string lName = XInfoConfig.NameToLowerName(typeInfo.name);
            if (!classNames.Contains(lName))
            {
                classNames.Add(lName);
            }
        }
        if (typeInfo.subInfos != null)
        {
            foreach (var sub in typeInfo.subInfos)
            {
                FindClass(sub, classNames);
            }
        }
    }
    public static void GenImports(FileFormatter f, XInfoConfig xinfoConfig)
    {
        var classNames = new List<string>();
        for (int i = 0; i < xinfoConfig.fields.Count; i++)
        {
            var fieldConfig = xinfoConfig.fields[i];

            FindClass(fieldConfig.typeInfo, classNames);
        }
        if (classNames.Count > 0)
        {
            foreach (string className in classNames)
            {
                f.TabPush($"import 'package:scene_hub/gen/{className}.dart';\n");
            }
            f.Push("\n");
        }
    }

    public static void GenFields(FileFormatter f, XInfoConfig xinfoConfig)
    {
        // char alpha = (char)0;
        for (int i = 0; i < xinfoConfig.fields.Count; i++)
        {
            var fieldConfig = xinfoConfig.fields[i];

            f.TabPush($"// [{i}]\n");
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
                f.TabPush("");
                f.PushDartToMsgPack(config.typeInfo, config.name);
                f.Push(",\n");
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
                f.Push(",\n");
            }
            f.AddTab(-1);
            f.PushTab().Push(");\n");
        }
        f.AddTab(-1);
        f.PushTab().Push("}\n");
    }
}