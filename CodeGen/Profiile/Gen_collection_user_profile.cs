using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Gen_collection_user_profile
{
    public static string Save(List<ProfileFieldConfig> configs)
    {
        var f = new FileFormatter();
        f.AddTab(2);
        for (int i = 0; i < configs.Count; i++)
        {
            var config = configs[i];
            // if (config.dataManagement == DataManagement.server ||
            //     config.dataManagement == DataManagement.server_client)
            {
                f.PushTab().Push(string.Format("if (profileNullable.{0} != null)", config.name)).PushLine();
                f.PushTab().Push("{").PushLine();
                f.AddTab(1);
                f.TabPush("var ");
                f.PushCopy_Db(config.typeInfo, config.name + "_Db", "profileNullable." + config.name, true, out bool canCompareNull);

                f.TabPush("var upd = {0}_Db != null\n".Format(config.name));
                f.AddTab(1);
                {
                    f.TabPush("? Builders<Profile_Db>.Update.Set(nameof(Profile_Db.{0}), {0}_Db)\n".Format(config.name));
                    f.TabPush(": Builders<Profile_Db>.Update.Unset(nameof(Profile_Db.{0}));\n".Format(config.name));
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