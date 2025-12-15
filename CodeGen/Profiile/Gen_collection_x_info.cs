using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Gen_collection_x_info
{
    public static string Save(XInfoConfig xinfoConfig)
    {
        List<XInfoFieldConfig> fields = xinfoConfig.fields;


        var f = new FileFormatter();
        f.AddTab(2);
        for (int i = 0; i < fields.Count; i++)
        {
            var config = fields[i];
            // if (config.dataManagement == DataManagement.server ||
            //     config.dataManagement == DataManagement.server_client)
            {
                f.PushTab().Push(string.Format("if (infoNullable.{0} != null)", config.name)).PushLine();
                f.PushTab().Push("{").PushLine();
                f.AddTab(1);
                f.TabPush("var ");
                f.PushCopy_Db(config.typeInfo, config.name + "_Db", "infoNullable." + config.name, true, out bool canCompareNull);

                f.TabPush("var upd = {0}_Db != null\n".Format(config.name));
                f.AddTab(1);
                {
                    f.TabPush("? Builders<{1}_Db>.Update.Set(nameof({1}_Db.{0}), {0}_Db)\n".Format(config.name, xinfoConfig.name));
                    f.TabPush(": Builders<{1}_Db>.Update.Unset(nameof({1}_Db.{0}));\n".Format(config.name, xinfoConfig.name));
                }
                f.AddTab(-1);

                f.PushTab().Push("updList.Add(upd);").PushLine();
                f.AddTab(-1);
                f.PushTab().Push("}").PushLine();
                f.PushLine();
            }
        }

        string str = f.GetString();
        return str;
    }
}