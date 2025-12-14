using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class Create_CallCreateIndex
{
    class VarObj
    {
        public string path;
        public FileFormatter ff;
    }
    public static void Create(List<ServerDataConfig> configs)
    {
        var list = new List<VarObj>();

        foreach (var config in configs)
        {
            string path = string.Format("Script/{0}/{1}.cs", config.dbFilesConfig.scriptFolder, config.dbFilesConfig.scriptOnStart);
            int index = list.FindIndex(_ => _.path == path);
            if (index < 0)
            {
                var ff = new FileFormatter();
                ff.AddTab(3);

                list.Add(new VarObj { path = path, ff = ff });
                index = list.Count - 1;
            }

            Create1(config, list[index]);
        }

        foreach (var _ in list)
        {
            XInfoProgram.ReplaceFile(_.path, new Mark[]
            {
                new Mark { startMark = "#region auto_callCreateIndex", text = _.ff.GetString() },
            });
        }
    }

    static void Create1(ServerDataConfig config, VarObj obj)
    {
        if (config.createCollectionCs && config.index.Count > 0)
        {
            obj.ff.TabPushF("await this.service.{0}{1}.CreateIndex();\n", config.className, config.postfix);
        }
    }
}