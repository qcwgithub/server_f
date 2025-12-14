using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Mark
{
    public string startMark;
    public string text;
}

public class ProfileProgram
{
    public static string ReplaceText(string content, string startMark, string endMark, string text)
    {
        int start = content.IndexOf(startMark);
        if (start < 0)
        {
            Console.WriteLine("startMark " + startMark + " not found");
            throw new Exception();
        }
        start = content.IndexOf('\n', start + 1);

        int end = content.IndexOf(endMark);
        if (end <= 0)
        {
            Console.WriteLine("endMark " + endMark + " not found");
            throw new Exception();
        }
        end = content.LastIndexOf('\n', end - 1);

        string pre = content.Substring(0, start + 1) + "\n";
        string post = content.Substring(end);
        return pre + text + post;
    }

    public static void ReplaceFile(string file, Mark[] array)
    {
        Console.WriteLine("ReplaceFile: " + file);
        string content = File.ReadAllText(file);
        for (int i = 0; i < array.Length; i++)
        {
            string endMark = array[i].startMark.Replace("#region", "#endregion");
            content = ReplaceText(content, array[i].startMark, endMark, array[i].text);
        }
        File.WriteAllText(file, content);
    }

    static void DoUserInfoStuff(ProfileConfig profileConfig)
    {
        List<ProfileFieldConfig> fields = profileConfig.fields;
        ReplaceFile("Data/Common/UserInfoNullable.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = GenProfileNullable.Do(fields) }
        });
        ReplaceFile("Data/Common/UserInfo_Db.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = GenProfile_Db.Do(profileConfig) }
        });

        ReplaceFile("Script/User/User_SaveUser.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = GenPMSavePlayer.Do(fields) }
        });

        /* ReplaceFile("Script/DBPlayer/table_player.cs", new Mark[]
        {
            new Mark { startMark = "#region autoVerifyCount", text = Gen_table_player.VerifyColumn_Count(fields) },
            new Mark { startMark = "#region autoVerifyColumn", text = Gen_table_player.VerifyColumn(fields) },
            new Mark { startMark = "#region autoDecode", text = Gen_table_player.Decode(fields) },
            new Mark { startMark = "#region autoInsertNames", text = Gen_table_player.Insert_Names(fields) },
            new Mark { startMark = "#region autoInsertValues", text = Gen_table_player.Insert_Values(fields) },
            new Mark { startMark = "#region autoSave", text = Gen_table_player.Save(fields) },
        }); */

        ReplaceFile("Script/Db/collections/collection_user_info.cs", new Mark[]
        {
            new Mark { startMark = "#region autoSave", text = Gen_collection_xxx_info.Save(profileConfig) },
        });
    }

    static FieldTypeInfo ReadTypeInfo(List<string> list, ref int index)
    {
        var info = new FieldTypeInfo();
        info.type = Enum.Parse<FieldType>(list[index++]);
        if (info.type.NeedConcreteString())
        {
            info.concreteString = list[index++];
        }

        int subCount = info.type.SubtypeCount();
        info.subInfos = new FieldTypeInfo[subCount];
        for (int i = 0; i < subCount; i++)
        {
            info.subInfos[i] = ReadTypeInfo(list, ref index);
        }
        info.CalcFieldTypeInfoName();
        info.CalcFieldTypeInfoName_Db();
        return info;
    }

    static void Do2()
    {
        Script.CsvHelper helper = Script.CsvUtils.Parse(CodeGen.Program.ReadAllText("CodeGen/ProfileXConfig.csv"));

        ProfileConfig profileConfig = null;
        List<ProfileConfig> list = new List<ProfileConfig>();
        while (helper.ReadRow())
        {
            string firstCol = helper.ReadString("type");
            if (firstCol.Length == 0)
            {
                continue;
            }

            if (firstCol.StartsWith("@"))
            {
                // start a new class
                profileConfig = new ProfileConfig();
                list.Add(profileConfig);
                profileConfig.name = firstCol.Substring(1);
                profileConfig.fields = new List<ProfileFieldConfig>();

                string s = helper.ReadString("ensureEx");
                profileConfig.ensureEx = s == "1" || s == "true";

                s = helper.ReadString("math");
                profileConfig.math = s == "1" || s == "true";

                s = helper.ReadString("createFromHelper");
                profileConfig.createFromHelper = s == "1" || s == "true";

                profileConfig.cache = helper.ReadString("cache");;

                if (profileConfig.cache == "redis")
                {
                    var c = new ProfileFieldConfig();

                    var list1 = new List<string>();
                    list1.Add("int_");
                    int i1 = 0;
                    c.typeInfo = ReadTypeInfo(list1, ref i1);

                    c.name = "isPlaceholder";

                    profileConfig.fields.Add(c);
                }

                continue;
            }
            else
            {
                var c = new ProfileFieldConfig();

                var list1 = new List<string>();
                list1.Add(helper.ReadString("type"));
                list1.Add(helper.ReadString("type2"));
                list1.Add(helper.ReadString("type3"));
                list1.Add(helper.ReadString("type4"));
                list1.Add(helper.ReadString("type5"));
                int i1 = 0;
                c.typeInfo = ReadTypeInfo(list1, ref i1);

                c.name = helper.ReadString("name");
                c.comment = helper.ReadString("comment");

                profileConfig.fields.Add(c);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            profileConfig = list[i];

            string text = @"using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{{
    public class {0}_Db : IIsDifferent_Db<{0}>
    {{
        #region auto
        #endregion auto
    }}
}}";
            File.WriteAllText("Data/Common/" + profileConfig.name + "_Db.cs", string.Format(text, profileConfig.name));


            // File.Copy("Data/Common/" + config.name + ".cs", "Data/Common/SCCommonData/" + config.name + "Nullable.cs", true);

            ReplaceFile("Data/Common/" + profileConfig.name + ".cs", new Mark[]
            {
                new Mark { startMark = "#region auto", text = GenProfile.Gen(profileConfig) },
            });

            ReplaceFile("Data/Common/" + profileConfig.name + "_Db.cs", new Mark[]
            {
                new Mark { startMark = "#region auto", text = GenProfile_Db.Do(profileConfig) },
            });

            if (profileConfig.name == "UserInfo")
            {
                DoUserInfoStuff(profileConfig);
            }
        }

    }

    public static void Do()
    {
        Do2();
    }
}