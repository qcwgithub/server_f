using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Gen_XInfo_Db
{
    public static void Do(XInfoConfig xinfoConfig)
    {
        var f = new FileFormatter();

        f.TabPush("using System.Collections.Generic;\n");
        f.TabPush("using MessagePack;\n");
        f.TabPush("using System.Numerics;\n");
        f.TabPush("using MongoDB.Bson.Serialization.Attributes;\n");
        f.Push("\n");
        f.TabPush("namespace Data\n");
        f.BlockStart();
        {
            f.TabPushF("public class {0}_Db : IIsDifferent_Db<{0}>\n", xinfoConfig.name);
            f.BlockStart();
            {
                for (int i = 0; i < xinfoConfig.fields.Count; i++)
                {
                    var config = xinfoConfig.fields[i];
                    f.PushTab().Push("[BsonIgnoreIfNull]").PushLine();
                    f.PushTab().Push(string.Format("public {0} {1};", config.typeInfo.ToDb(), config.name)).PushLine();
                }

                f.Push("\n");
                Gen_DeepCopyFrom_Db(f, xinfoConfig);
            }
            f.BlockEnd();
        }
        f.BlockEnd();

        File.WriteAllText("Data/Common/Gen/" + xinfoConfig.name + "_Db.cs", f.GetString());
    }

    public static void Gen_DeepCopyFrom_Db(FileFormatter f, XInfoConfig xinfoConfig)
    {
        f.PushTab().Push("public bool DeepCopyFrom(", xinfoConfig.name, " other)\n");
        f.PushTab().Push("{\n");
        f.AddTab(1);

        f.TabPush("bool empty = true;\n\n");

        List<string> temp = new List<string>();
        for (int i = 0; i < xinfoConfig.fields.Count; i++)
        {
            var config = xinfoConfig.fields[i];
            f.PushTab();
            f.PushCopy_Db(config.typeInfo, "this." + config.name, "other." + config.name, false, out bool canCompareNull);
            if (canCompareNull)
            {
                f.TabPush("if (this.{0} != null)\n".Format(config.name));
                f.BlockStart();
            }
            f.TabPush("empty = false;\n");
            if (canCompareNull)
            {
                f.BlockEnd();
            }
            // f.TabPush("this.{0} = XInfoHelper_Db.Copy(other.{0});\n".Format(config.name));
            f.Push("\n");
        }
        f.TabPush("return !empty;\n");

        f.AddTab(-1);
        f.PushTab().Push("}\n");
    }
}