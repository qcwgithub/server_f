using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Gen_XInfo_Db
{
    public static string Do(XInfoConfig xinfoConfig)
    {
        var f = new FileFormatter();

        f.AddTab(2);
        // char alpha = (char)0;
        for (int i = 0; i < xinfoConfig.fields.Count; i++)
        {
            var config = xinfoConfig.fields[i];
            // if (config.dataManagement == DataManagement.server ||
            //     config.dataManagement == DataManagement.server_client)
            {
                // f.PushTab().Push(string.Format("[Key({0})]", i)).PushLine();
                f.PushTab().Push("[BsonIgnoreIfNull]").PushLine();
                f.PushTab().Push(string.Format("public {0} {1};", config.typeInfo.ToDb(), config.name)).PushLine();
            }
        }

        // f.Push("\n");
        // GenAllDefaults(f, xinfoConfig);

        // f.Push("\n");
        // MakeDefaultsNull(f, xinfoConfig);

        f.Push("\n");
        Gen_DeepCopyFrom_Db(f, xinfoConfig);

        string str = f.GetString();
        return str;
    }

    // public static void GenAllDefaults(FileFormatter f, XInfoConfig xinfoConfig)
    // {
    //     // if (xinfoConfig.addLastDiffField)
    //     // {
    //     //     f.PushTab().Push("[IgnoreMember]\n");
    //     //     f.PushTab().Push("public string lastDiffField;\n");
    //     // }

    //     f.PushTab().Push("public bool AllDefaults()\n");

    //     f.PushTab().Push("{\n");
    //     f.AddTab(1);

    //     for (int i = 0; i < xinfoConfig.fields.Count; i++)
    //     {
    //         var config = xinfoConfig.fields[i];

    //         f.TabPush("if ({0})\n".Format(config.typeInfo.IsDefault("this." + config.name, false)));
    //         f.BlockStart();
    //         {
    //             f.TabPush("return false;\n");
    //         }
    //         f.BlockEnd();
    //     }

    //     f.PushTab().Push("return true;\n");

    //     f.AddTab(-1);
    //     f.PushTab().Push("}\n");
    //     f.Push("\n");
    // }

    // public static void MakeDefaultsNull(FileFormatter f, XInfoConfig xinfoConfig)
    // {
    //     // if (xinfoConfig.addLastDiffField)
    //     // {
    //     //     f.PushTab().Push("[IgnoreMember]\n");
    //     //     f.PushTab().Push("public string lastDiffField;\n");
    //     // }

    //     f.PushTab().Push("public void MakeDefaultsNull()\n");

    //     f.PushTab().Push("{\n");
    //     f.AddTab(1);

    //     for (int i = 0; i < xinfoConfig.fields.Count; i++)
    //     {
    //         var config = xinfoConfig.fields[i];
    //         f.TabPush("if ({0})\n".Format(config.typeInfo.IsDefault("this." + config.name, true)));
    //         f.BlockStart();
    //         {
    //             f.PushTab().Push("this." + config.name, " = null;\n");
    //         }
    //         f.BlockEnd();
    //         f.TabPush("else\n");
    //         f.BlockStart();
    //         {
    //             f.PushMakeDefaultsNull(config.typeInfo, "this." + config.name);
    //         }
    //         f.BlockEnd();
    //     }

    //     f.AddTab(-1);
    //     f.PushTab().Push("}\n");
    //     f.Push("\n");
    // }

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