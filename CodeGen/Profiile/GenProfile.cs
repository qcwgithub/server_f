using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

// Profile.cs
// ProfileXX.cs
// 共用
public class GenProfile
{
    public static string Gen(ProfileConfig profileConfig)
    {
        var f = new FileFormatter();

        f.AddTab(2);

        GenFields(f, profileConfig);

        f.Push("\n");
        GenEnsures(f, profileConfig);

        f.Push("\n");
        GenIsDifferent_DeepCopyFrom(f, profileConfig);

        if (profileConfig.math)
        {
            f.Push("\n");
            GenMath(f, profileConfig);
        }

        if (profileConfig.createFromHelper)
        {
            f.Push("\n");
            GenCreateFromHelper(f, profileConfig);
        }

        string str = f.GetString();
        return str;
    }

    public static void GenFields(FileFormatter f, ProfileConfig profileConfig)
    {
        // char alpha = (char)0;
        for (int i = 0; i < profileConfig.fields.Count; i++)
        {
            var fieldConfig = profileConfig.fields[i];

            f.PushTab().Push(string.Format("[Key({0})]", i)).PushLine();
            f.PushTab().Push("public ").Push(fieldConfig.typeInfo.name).Push(" ").Push(fieldConfig.name).Push(";");

            if (!string.IsNullOrEmpty(fieldConfig.comment))
            {
                f.Push(" // ", fieldConfig.comment);
            }

            f.Push("\n");
        }
    }
    public static void GenEnsures(FileFormatter f, ProfileConfig profileConfig)
    {
        // static Ensure
        f.PushTab().Push("public static ", profileConfig.name, " Ensure(", profileConfig.name, "? p)\n");
        f.PushTab().Push("{\n");
        f.AddTab(1);
        {
            f.PushTab().Push("if (p == null)\n");
            f.PushTab().Push("{\n");
            f.AddTab(1);
            {
                f.PushTab().Push("p = new ", profileConfig.name, "();\n");
            }
            f.AddTab(-1);
            f.PushTab().Push("}\n");

            f.PushTab().Push("p.Ensure();\n");
            f.PushTab().Push("return p;\n");
        }
        f.AddTab(-1);
        f.PushTab().Push("}\n\n");

        // instance Ensure
        f.PushTab().Push("public void Ensure()\n");
        f.PushTab().Push("{\n");
        f.AddTab(1);
        {
            for (int i = 0; i < profileConfig.fields.Count; i++)
            {
                var config = profileConfig.fields[i];
                f.PushEnsure(config.typeInfo, "this." + config.name, "this." + config.name);
            }

            if (profileConfig.ensureEx)
            {
                f.TabPush("\n");
                f.TabPush("this.EnsureEx();\n");
            }
        }
        f.AddTab(-1);
        f.PushTab().Push("}\n");
    }

    public static void GenIsDifferent_DeepCopyFrom(FileFormatter f, ProfileConfig profileConfig)
    {
        // if (profileConfig.addLastDiffField)
        // {
        //     f.PushTab().Push("[IgnoreMember]\n");
        //     f.PushTab().Push("public string lastDiffField;\n");
        // }

        f.PushTab().Push("public bool IsDifferent(", profileConfig.name, " other)\n");//, List<string> tracker = null", ")\n");

        f.PushTab().Push("{\n");
        f.AddTab(1);

        for (int i = 0; i < profileConfig.fields.Count; i++)
        {
            var config = profileConfig.fields[i];

            f.TabPush("if ({0})\n".Format(
                config.typeInfo.ToIsDifferent("this." + config.name, "other." + config.name)));
            f.BlockStart();
            {
                // if (profileConfig.addLastDiffField)
                //     f.TabPush("this.lastDiffField = \"", config.name, "\";\n");

                f.TabPush("return true;\n");
            }
            f.BlockEnd();
        }

        f.PushTab().Push("return false;\n");

        f.AddTab(-1);
        f.PushTab().Push("}\n");
        f.Push("\n");

        //-------------------------------------------------
        f.PushTab().Push("public void DeepCopyFrom(", profileConfig.name, " other)\n");
        f.PushTab().Push("{\n");
        f.AddTab(1);

        for (int i = 0; i < profileConfig.fields.Count; i++)
        {
            var config = profileConfig.fields[i];
            f.TabPush(config.typeInfo.ToDeepCopyFrom("this." + config.name, "other." + config.name), ";\n");
        }

        f.AddTab(-1);
        f.PushTab().Push("}\n");
    }

    static string[] mathOps = new string[]
    {
        "Add_*_1",
        "Sub_*_1",
        "Mul_*_1",
        "Div_*_1",

        "Add_1_1",
        "Sub_1_1",
        // "Mul_1",
        // "Div_1",

        "Add_*_*",
        "Sub_*_*",
        // "Mul",
        // "Div",
        "Get_1_0",
        "Set_1_1"
    };
    public static void GenMath(FileFormatter f, ProfileConfig profileConfig)
    {
        if (profileConfig.fields.Count == 0)
        {
            return;
        }

        FieldTypeInfo typeInfo0 = profileConfig.fields[0].typeInfo;
        if (!typeInfo0.type.SupportsMath())
        {
            return;
        }

        bool allSame = true;
        for (int j = 1; j < profileConfig.fields.Count; j++)
        {
            FieldTypeInfo typeInfoj = profileConfig.fields[j].typeInfo;
            if (typeInfoj.type != typeInfo0.type)
            {
                allSame = false;
                break;
            }
        }

        for (int i = 0; i < mathOps.Length; i++)
        {
            string[] array = mathOps[i].Split('_');
            string methodName = array[0];

            bool allFields = array[1] == "*";
            bool singleField = !allFields;

            bool allInput = array[2] == "*";
            bool singleInput = array[2] == "1";
            bool zeroInput = array[2] == "0";

            if ((singleField || singleInput) && !allSame)
            {
                continue;
            }

            f.PushTab();
            f.Push("public ");
            switch (methodName)
            {
                case "Get":
                    f.Push(typeInfo0.CalcFieldTypeInfoName());
                    break;
                default:
                    f.Push("void");
                    break;
            }
            f.Push(" ", methodName, "(");
            if (singleField)
            {
                f.Push("string name");
            }
            if (singleInput)
            {
                if (singleField)
                {
                    f.Push(", ");
                }
                f.Push(typeInfo0.CalcFieldTypeInfoName(), " v");
            }
            else if (allInput)
            {
                f.Push(profileConfig.name, " other");
            }
            f.Push(")\n");

            f.PushTab().Push("{\n");
            f.AddTab(1);
            {
                if (singleField)
                {
                    f.PushTab().Push("switch (name)\n");
                    f.PushTab().Push("{\n");
                    f.AddTab(1);
                }

                foreach (ProfileFieldConfig fieldConfig in profileConfig.fields)
                {
                    if (singleField)
                    {
                        f.PushTab().Push("case nameof(this.", fieldConfig.name, "):\n");
                        f.AddTab(1);
                    }

                    f.PushTab();
                    switch (methodName)
                    {
                        case "Add": f.Push("this.", fieldConfig.name, " += "); break;
                        case "Sub": f.Push("this.", fieldConfig.name, " -= "); break;
                        case "Mul": f.Push("this.", fieldConfig.name, " *= "); break;
                        case "Div": f.Push("this.", fieldConfig.name, " /= "); break;
                        case "Set": f.Push("this.", fieldConfig.name, " = "); break;
                        case "Get": f.Push("return this.", fieldConfig.name); break;
                    }

                    if (singleInput)
                    {
                        f.Push("v");
                    }
                    else if (allInput)
                    {
                        f.Push("other." + fieldConfig.name);
                    }

                    f.Push(";\n");

                    if (singleField)
                    {
                        switch (methodName)
                        {
                            case "Get":
                                break;
                            default:
                                f.PushTab().Push("break;\n");
                                break;
                        }
                        f.AddTab(-1);
                    }
                }

                if (singleField)
                {
                    f.PushTab().Push("default:\n");
                    f.AddTab(1);

                    f.PushTab().Push("throw new Exception($\"field not exist: ", profileConfig.name, ".{name}\");\n");
                    f.AddTab(-1);
                }

                if (singleField)
                {
                    f.AddTab(-1);
                    f.PushTab().Push("}\n");
                }
            }
            f.AddTab(-1);
            f.PushTab().Push("}\n");

            if (i < mathOps.Length - 1)
            {
                f.Push("\n");
            }
        }
    }

    public static void GenCreateFromHelper(FileFormatter f, ProfileConfig profileConfig)
    {
        f.PushTab().Push("public static ", profileConfig.name, " Create(CsvHelper helper)\n");
        f.PushTab().Push("{\n");
        f.AddTab(1);

        f.PushTab().Push("var self = Ensure(null);\n");
        f.Push("\n");

        foreach (ProfileFieldConfig fieldConfig in profileConfig.fields)
        {
            f.PushTab().Push("self.", fieldConfig.name, " = helper.", fieldConfig.typeInfo.HelperRead(), "(\"", fieldConfig.name, "\");\n");
        }
        f.Push("\n");
        f.PushTab().Push("return self;\n");

        f.AddTab(-1);
        f.PushTab().Push("}\n");
        f.Push("\n");
    }
}