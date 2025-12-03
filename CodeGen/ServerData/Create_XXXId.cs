using System.Collections.Generic;
using System.IO;
using System.Linq;
public class Create_XXXId
{
    public static void Create(Dictionary<string, ServerDataCopyConfig> dict)
    {
        foreach (var kv in dict)
        {
            ServerDataCopyConfig config = kv.Value;

            var ff = new FileFormatter();

            ff.TabPush("namespace Data\n");
            ff.BlockStart();
            {
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("public static class {0}\n", config.name);
                ff.BlockStart();
                {
                    for (int i = 0; i < config.count; i++)
                    {
                        ff.TabPushF("public const int Id_{0} = {0};\n", i + 1);
                    }
                    ff.TabPush("public static int[] All = new int[] {");
                    for (int i = 0; i < config.count; i++)
                    {
                        ff.Push(" Id_" + (i + 1));
                        if (i < config.count - 1)
                        {
                            ff.Push(",");
                        }
                    }
                    ff.Push(" };\n");
                    ff.TabPush("public const int InvalidId = 0;\n");

                    ff.TabPush("\n");

                    ff.TabPush("public static bool IsDefault(int id)\n");
                    ff.BlockStart();
                    {
                        ff.TabPushF("return id == All[0];\n");
                    }
                    ff.BlockEnd();

                    ff.TabPush("\n");

                    ff.TabPush("public static bool IsValid(int id)\n");
                    ff.BlockStart();
                    {
                        ff.TabPushF("for (int i = 0; i < All.Length; i++)\n");
                        ff.BlockStart();
                        {
                            ff.TabPushF("if (id == All[i])\n");
                            ff.BlockStart();
                            ff.TabPush("return true;\n");
                            ff.BlockEnd();
                        }
                        ff.BlockEnd();
                    }
                    ff.TabPush("return false;\n");
                    ff.BlockEnd();

                    

                    ff.TabPush("\n");

                    ff.TabPush("public static bool AssertIsValid(int id)\n");
                    ff.BlockStart();
                    {
                        ff.TabPushF("if (!IsValid(id))\n");
                        ff.BlockStart();
                        ff.TabPush("MyDebug.Assert(false);\n");
                        ff.TabPush("return false;\n");
                        ff.BlockEnd();
                        ff.TabPush("return true;\n");
                    }
                    ff.BlockEnd();

                    ff.TabPush("\n");

                    ff.TabPush("public static int ToIndex(int id)\n");
                    ff.BlockStart();
                    {
                        ff.TabPushF("return id - All[0];\n");
                    }
                    ff.BlockEnd();
                }
                ff.BlockEnd();
            }
            ff.BlockEnd();

            File.WriteAllText($"server/Data/Common/SCCommonData/{config.name}.cs", ff.GetString());
        }
    }
}