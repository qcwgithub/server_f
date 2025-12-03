using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class GenPMSavePlayer
{
    public static string Do(List<ProfileFieldConfig> configs)
    {
        var f = new FileFormatter();

        f.AddTab(3);
        // char alpha = (char)0;
        for (int i = 0; i < configs.Count; i++)
        {
            var config = configs[i];
            // if (config.dataManagement == DataManagement.server ||
            //     config.dataManagement == DataManagement.server_client)
            {
                string accessThis = "last." + config.name;
                string accessOther = "curr." + config.name;

                f.TabPush("if ({0})\n".Format(
                    config.typeInfo.ToIsDifferent(accessThis, accessOther, false)));
                f.BlockStart();
                {
                    f.TabPush("profileNullable.{0} = {1};\n".Format(config.name, accessOther));
                    f.TabPush(config.typeInfo.ToDeepCopyFrom(accessThis, accessOther), ";\n");
                    f.TabPush("if (buffer == null) buffer = new List<string>();\n");
                    f.TabPush("buffer.Add(\"{0}\");\n".Format(config.name));
                }
                f.BlockEnd();
            }
        }

        string str = f.GetString();
        return str;
    }
}