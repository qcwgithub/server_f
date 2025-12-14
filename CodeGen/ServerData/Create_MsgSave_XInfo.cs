using System;
using System.IO;

public class Create_MsgSave_XInfo
{
    public static void Create(ServerDataConfig config)
    {
        string dir = "Data/Common/Messages/";
        Directory.CreateDirectory(dir);
        if (config.save.Count > 0)
        {
            string content = Create1(config);

            string path = string.Format("{0}MsgSave_{1}.cs", dir, config.xinfoType);
            File.WriteAllText(path, content);
        }
    }

    static string Create1(ServerDataConfig config)
    {
        FileFormatter ff = new FileFormatter();
        // ff.TabPush("// Auto created, DO NOT modify it manually\n\n");

        ff.TabPush("using System.Collections.Generic;\n");
        ff.TabPush("using MessagePack;\n");
        ff.Push("\n");

        ff.TabPush("namespace Data\n");
        ff.BlockStart();
        {
            config.PushUsingOriginal(ff);

            ff.TabPush("//// AUTO CREATED ////\n");
            ff.TabPush("[MessagePackObject]\n");
            ff.TabPush(string.Format("public sealed class MsgSave_{0}\n", config.xinfoType));
            ff.BlockStart();
            {
                ff.TabPush("[Key(0)]\n");
                ff.TabPushF("public {0} info;\n", config.xinfoType);
            }
            ff.BlockEnd();

            ff.TabPush("\n");
            ff.TabPush("//// AUTO CREATED ////\n");
            ff.TabPush("[MessagePackObject]\n");
            ff.TabPush(string.Format("public sealed class ResSave_{0}\n", config.xinfoType));
            ff.BlockStart();
            {

            }
            ff.BlockEnd();
        }
        ff.BlockEnd();

        return ff.GetString();
    }
}