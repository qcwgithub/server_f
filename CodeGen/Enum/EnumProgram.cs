
public class EnumProgram
{
    public static void MainX()
    {
        Script.CsvHelper helper = Script.CsvUtils.Parse(CodeGen.Program.ReadAllText("CodeGen/EnumConfig.csv"));

        EnumConfig config = null;
        List<EnumConfig> list = new List<EnumConfig>();
        while (helper.ReadRow())
        {
            string firstCol = helper.ReadString("name");
            if (firstCol.Length == 0)
            {
                continue;
            }

            if (firstCol.StartsWith("@"))
            {
                // start a new class
                config = new EnumConfig();
                list.Add(config);
                config.name = firstCol.Substring(1);
                config.fields = new List<EnumFieldConfig>();

                continue;
            }
            else
            {
                var c = new EnumFieldConfig();
                c.name = helper.ReadString("name");
                c.value = helper.ReadInt("value");
                c.dartDefault = helper.ReadBool("dartDefault");
                config.fields.Add(c);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            config = list[i];

            //cs
            {
                var f = new FileFormatter();
                f.TabPush("namespace Data\n");
                f.BlockStart();
                {
                    f.TabPushF("public enum {0}\n", config.name);
                    f.BlockStart();
                    {
                        foreach (var field in config.fields)
                        {
                            f.TabPushF("{0} = {1},", field.name, field.value);
                            if (field.dartDefault)
                            {
                                f.Push(" // default for dart");
                            }
                            f.Push("\n");
                        }
                    }
                    f.BlockEnd();
                }
                f.BlockEnd();

                File.WriteAllText("Data/Common/Gen/" + config.name + ".cs", f.GetString());
            }

            // dart
            {
                var f = new FileFormatter();
                f.SetTabWidth2();
                f.TabPushF("enum {0} {{\n", config.name);
                f.AddTab(1);
                {
                    for (int j = 0; j < config.fields.Count; j++)
                    {
                        var field = config.fields[j];
                        f.TabPushF("{0}({1})", XInfoConfig.FirstCharacterLower(field.name), field.value);
                        f.Push(j == config.fields.Count - 1 ? ";" : ",");
                        if (field.dartDefault)
                        {
                            f.Push(" // default for dart");
                        }
                        f.Push("\n");
                    }
                    f.Push("\n");

                    f.TabPushF("static {0} fromCode(int code) {{\n", config.name);
                    f.AddTab(1);
                    {
                        f.TabPush("switch (code) {\n");
                        f.AddTab(1);
                        {
                            for (int j = 0; j < config.fields.Count; j++)
                            {
                                var field = config.fields[j];
                                f.TabPushF("case {0}:\n", field.value);
                                f.AddTab(1);
                                f.TabPushF("return {0}.{1};\n", config.name, XInfoConfig.FirstCharacterLower(field.name));
                                f.AddTab(-1);
                            }
                            f.TabPush("default:\n");
                            f.AddTab(1);
                            f.TabPushF("return {0}.{1};\n", config.name, XInfoConfig.FirstCharacterLower(
                                config.fields.Find(x => x.dartDefault).name
                            ));
                            f.AddTab(-1);
                        }
                        f.AddTab(-1);
                        f.TabPush("}\n");
                    }
                    f.AddTab(-1);
                    f.TabPush("}\n");
                    f.Push("\n");

                    f.TabPush("final int code;\n");
                    f.TabPushF("const {0}(this.code);\n", config.name);
                }
                f.AddTab(-1);
                f.TabPush("}");

                File.WriteAllText("../client_f/lib/gen/" + XInfoConfig.NameToLowerName(config.name) + ".dart",
                    f.GetString());
            }
        }
    }

    public static void CreateOne()
    {
        
    }
}