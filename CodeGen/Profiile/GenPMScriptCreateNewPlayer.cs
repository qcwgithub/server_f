using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class GenPMScriptCreateNewPlayer
{
    public static string Do(List<ProfileFieldConfig> configs)
    {
        var f = new FileFormatter();

        f.AddTab(3);
        // char alpha = (char)0;
        for (int i = 0; i < configs.Count; i++)
        {
            ProfileFieldConfig config = configs[i];
            f.PushTab();
            if (string.IsNullOrEmpty(config.defaultValueExp))
            {
                f.Push("// profile.", config.name);
            }
            else
            {
                f.Push("profile.", config.name, " = ", config.defaultValueExp, ";");
            }
            f.Push("\n");
        }

        string str = f.GetString();
        return str;
    }
}