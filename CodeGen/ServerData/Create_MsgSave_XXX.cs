using System;
using System.IO;

public class Create_MsgSave_XXX
{
    public static void Create(ServerDataConfig config)
    {
        string dir = "Data/Common/Messages/";
        Directory.CreateDirectory(dir);
        foreach (var query in config.query)
        {
            string content = Create1(config, query);

            string path = string.Format("{0}MsgSave_{1}.cs", dir, config.profileType);
            File.WriteAllText(path, content);
        }
    }

    static string Create1(ServerDataConfig config, ServerDataConfig.Query query)
    {
        FileFormatter ff = new FileFormatter();
        // ff.TabPush("// Auto created, DO NOT modify it manually\n\n");

        ff.TabPush("using System.Collections.Generic;\n");
        ff.TabPush("using MessagePack;\n");
        ff.TabPush("using longid = System.Int64;\n");
        ff.Push("\n");

        ff.TabPush("namespace Data\n");
        ff.BlockStart();
        {
            config.PushUsingOriginal(ff);

            ff.TabPush("//// AUTO CREATED ////\n");
            ff.TabPush("[MessagePackObject]\n");
            ff.TabPush(string.Format("public sealed class MsgSave_{0}\n", config.profileType));
            ff.BlockStart();
            {
                ff.TabPush("[Key(0)]\n");
                ff.TabPushF("public {0} info;\n", config.profileType);
            }
            ff.BlockEnd();

            ff.TabPush("\n");
            ff.TabPush("//// AUTO CREATED ////\n");
            ff.TabPush("[MessagePackObject]\n");
            ff.TabPush(string.Format("public sealed class ResSave_{0}\n", config.profileType));
            ff.BlockStart();
            {

            }
            ff.BlockEnd();
        }
        ff.BlockEnd();

        return ff.GetString();
    }
}