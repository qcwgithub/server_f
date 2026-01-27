
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
                            f.TabPushF("{0} = {1},\n", field.name, field.value);
                        }
                    }
                    f.BlockEnd();
                }
                f.BlockEnd();

                File.WriteAllText("Data/Common/Gen/" + config.name + ".cs", f.GetString());
            }

            // dart
            {

            }
        }
    }
}