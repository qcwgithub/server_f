using System;
using System.IO;

public class Create_MsgQuery_XXX
{
    public static void Create(ServerDataConfig config)
    {
        string dir = "Data/Common/Messages/";
        Directory.CreateDirectory(dir);
        foreach (var query in config.query)
        {
            string content = Create1(config, query);

            string path = string.Format("{0}Msg{1}.cs", dir, query.methodName);
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
            ff.TabPush(string.Format("public sealed class Msg{0}\n", query.methodName));
            ff.BlockStart();
            {
                for (int e = 0; e < query.methodParamExps.Length; e++)
                {
                    ff.TabPush("[Key(", e.ToString(), ")]\n");
                    ff.TabPush("public ", query.methodParamExps[e], ";\n");
                }
            }
            ff.BlockEnd();

            ff.TabPush("\n");
            ff.TabPush("//// AUTO CREATED ////\n");
            ff.TabPush("[MessagePackObject]\n");
            ff.TabPush(string.Format("public sealed class Res{0}\n", query.methodName));
            ff.BlockStart();
            if (!string.IsNullOrEmpty(query.returnExp))
            {
                ff.TabPush("[Key(0)]\n");
                ff.TabPush("public ", query.returnExp, " result;\n");
            }
            ff.BlockEnd();
        }
        ff.BlockEnd();

        return ff.GetString();
    }
}