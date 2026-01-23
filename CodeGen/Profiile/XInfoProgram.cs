using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Mark
{
    public string startMark;
    public string text;
}

public class XInfoProgram
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

    static void DoXInfoStuff(XInfoConfig xinfoConfig)
    {
        string x = xinfoConfig.name.Substring(0, xinfoConfig.name.IndexOf("Info"));
        string x_lower = x.ToLower();

        List<XInfoFieldConfig> fields = xinfoConfig.fields;
        ReplaceFile($"Data/Common/{xinfoConfig.name}Nullable.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = Gen_XInfoNullable.Do(fields) }
        });
        ReplaceFile($"Data/Common/{xinfoConfig.name}_Db.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = Gen_XInfo_Db.Do(xinfoConfig) }
        });

        ReplaceFile($"Script/{x}/{x}Service.Save{x}.cs", new Mark[]
        {
            new Mark { startMark = "#region auto", text = Gen_X_SaveX.Do(fields) }
        });

        ReplaceFile($"Script/Db/collections/collection_{x_lower}_info.manual.cs", new Mark[]
        {
            new Mark { startMark = "#region autoSave", text = Gen_collection_x_info.Save(xinfoConfig) },
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

    public static void MainX()
    {
        Script.CsvHelper helper = Script.CsvUtils.Parse(CodeGen.Program.ReadAllText("CodeGen/XInfoConfig.csv"));

        XInfoConfig xinfoConfig = null;
        List<XInfoConfig> list = new List<XInfoConfig>();
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
                xinfoConfig = new XInfoConfig();
                list.Add(xinfoConfig);
                xinfoConfig.name = firstCol.Substring(1);
                xinfoConfig.fields = new List<XInfoFieldConfig>();

                string s = helper.ReadString("ensureEx");
                xinfoConfig.ensureEx = s == "1" || s == "true";

                s = helper.ReadString("math");
                xinfoConfig.math = s == "1" || s == "true";

                s = helper.ReadString("createFromHelper");
                xinfoConfig.createFromHelper = s == "1" || s == "true";

                xinfoConfig.cacheType = helper.ReadEnum<CacheType>("cacheType");

                if (xinfoConfig.cacheType == CacheType.Redis)
                {
                    var c = new XInfoFieldConfig();

                    var list1 = new List<string>();
                    list1.Add("int_");
                    int i1 = 0;
                    c.typeInfo = ReadTypeInfo(list1, ref i1);

                    c.name = "isPlaceholder";

                    xinfoConfig.fields.Add(c);
                }

                continue;
            }
            else
            {
                var c = new XInfoFieldConfig();

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

                xinfoConfig.fields.Add(c);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            xinfoConfig = list[i];

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
            File.WriteAllText("Data/Common/" + xinfoConfig.name + "_Db.cs", string.Format(text, xinfoConfig.name));


            // File.Copy("Data/Common/" + config.name + ".cs", "Data/Common/SCCommonData/" + config.name + "Nullable.cs", true);


            ReplaceFile("Data/Common/" + xinfoConfig.name + "_Db.cs", new Mark[]
            {
                new Mark { startMark = "#region auto", text = Gen_XInfo_Db.Do(xinfoConfig) },
            });

            text = @"using MessagePack;

namespace Data
{{
    [MessagePackObject]
    public class {0}{1}
    {{
        #region auto
        #endregion auto
    }}
}}";
            File.WriteAllText("Data/Common/" + xinfoConfig.name + ".cs", string.Format(text, xinfoConfig.name,
                xinfoConfig.cacheType == CacheType.Redis ? " : ICanBePlaceholder" : string.Empty));
            ReplaceFile("Data/Common/" + xinfoConfig.name + ".cs", new Mark[]
            {
                new Mark { startMark = "#region auto", text = GenXInfo.Gen(xinfoConfig) },
            });

            if (xinfoConfig.cacheType == CacheType.Memory)
            {
                DoXInfoStuff(xinfoConfig);
            }
        }
    }
}