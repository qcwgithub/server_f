using System;
using System.Collections.Generic;

public class Create_DBXXService_var
{
    class VarObj
    {
        public string path;
        public FileFormatter varDecl;
        public FileFormatter varCreate;
        public FileFormatter handlerCreate;

        public Dictionary<string, EntryVar> entryVarDict;
    }

    public class EntryVar
    {
        public FileFormatter decl;
        public FileFormatter create;
        public FileFormatter copyGetVar;

        public EntryVar()
        {
            this.decl = new FileFormatter();
            this.decl.AddTab(2);

            this.create = new FileFormatter();
            this.create.AddTab(3);

            this.copyGetVar = new FileFormatter();
            this.copyGetVar.AddTab(2);
        }
    }

    public static void Create(List<ServerDataConfig> configs)
    {
        var list = new List<VarObj>();

        var entryVarDict = new Dictionary<string, EntryVar>();
        foreach (string dbName in ServerDataConfig.c_dbNames)
        {
            entryVarDict[dbName] = new EntryVar();
        }

        foreach (var config in configs)
        {
            string path = string.Format("Script/{0}/{1}.cs", config.dbFilesConfig.scriptFolder, config.dbFilesConfig.serviceClassName);

            int index = list.FindIndex(_ => _.path == path);
            if (index < 0)
            {
                var decl = new FileFormatter();
                decl.AddTab(2);

                var create = new FileFormatter();
                create.AddTab(3);

                var handlerCreate = new FileFormatter();
                handlerCreate.AddTab(3);

                list.Add(new VarObj { path = path, varDecl = decl, varCreate = create, handlerCreate = handlerCreate, entryVarDict = entryVarDict });
                index = list.Count - 1;
            }

            Create1(config, list[index]);
        }

        for (int i = 0; i < configs.Count; i++)
        {
            var config0 = configs[i];
            if (config0.copy == null)
            {
                continue;
            }

            if (config0.copy.index != 0)
            {
                throw new Exception();
            }

            var copyGetVar = entryVarDict[config0.dbName].copyGetVar;

            copyGetVar.TabPushF("public I{0}Proxy {1}Proxy(int id)\n", config0.xinfoType, CodeGen.Program.FirstCharacterToLowercase(config0.xinfoType));
            copyGetVar.BlockStart();
            {
                for (int j = 0; j < config0.copy.config.count; j++)
                {
                    copyGetVar.TabPushF("if (id == {0}.Id_{1})\n", config0.copy.config.name, j + 1);
                    copyGetVar.BlockStart();
                    {
                        copyGetVar.TabPushF("return this._{0}Proxy{1};\n", CodeGen.Program.FirstCharacterToLowercase(configs[i + j].xinfoType), configs[i + j].postfix);
                    }
                    copyGetVar.BlockEnd();
                }
                copyGetVar.TabPushF("else\n");
                copyGetVar.BlockStart();
                {
                    copyGetVar.TabPush("return null;\n");
                }
                copyGetVar.BlockEnd();
            }
            copyGetVar.BlockEnd();

            i += config0.copy.config.count - 1;
        }


        foreach (var _ in list)
        {
            XInfoProgram.ReplaceFile(_.path, new Mark[]
            {
                new Mark { startMark = "#region auto_collection_var_decl", text = _.varDecl.GetString() },
                new Mark { startMark = "#region auto_collection_var_create", text = _.varCreate.GetString() },
            });
        }

        foreach (string dbName in ServerDataConfig.c_dbNames)
        {
            var entryVar = entryVarDict[dbName];
            XInfoProgram.ReplaceFile(ServerDataConfig.s_dbFilesConfigDict[dbName].server_path, new Mark[]
            {
                new Mark{ startMark = "#region auto_proxy_var_decl", text = entryVar.decl.GetString() },
                new Mark{ startMark = "#region auto_proxy_copy_get_var", text = entryVar.copyGetVar.GetString() },
                new Mark{ startMark = "#region auto_proxy_var_create", text = entryVar.create.GetString() },
            });
        }
    }

    static void Create1(ServerDataConfig config, VarObj obj)
    {
        if (config.createCollectionCs)
        {
            obj.varDecl.TabPushF("public {0}{1} {0}{1};\n", config.fileName, config.postfix);
            obj.varCreate.TabPushF("this.{0}{1} = new {0}{1}({2}, this);\n", config.fileName, config.postfix, config.dbFilesConfig.server_var);
        }

        foreach (var save in config.save)
        {
            obj.handlerCreate.TabPushF("this.dispatcher.AddHandler(new Save_{0}{1}({2}, this));\n", config.xinfoType, config.postfix, config.dbFilesConfig.server_var);
        }

        foreach (var query in config.query)
        {
            obj.handlerCreate.TabPushF("this.dispatcher.AddHandler(new {0}{1}({2}, this));\n", query.methodName, config.postfix, config.dbFilesConfig.server_var);
        }

        if (config.cacheType.IsCreateProxy())
        {
            if (config.copy == null)
            {
                obj.entryVarDict[config.dbName].decl.TabPushF("public {0}Proxy{1} {2}Proxy{1} {{ get; private set; }}\n",
                    config.xinfoType,
                    config.postfix,
                    CodeGen.Program.FirstCharacterToLowercase(config.xinfoType));

                obj.entryVarDict[config.dbName].create.TabPushF("this.{0}Proxy{1} = new {2}Proxy{1}(this);\n",
                    CodeGen.Program.FirstCharacterToLowercase(config.xinfoType),
                    config.postfix,
                    config.xinfoType);
            }
            else
            {
                obj.entryVarDict[config.dbName].decl.TabPushF("private {0}Proxy{1} _{2}Proxy{1};\n",
                    config.xinfoType,
                    config.postfix,
                    CodeGen.Program.FirstCharacterToLowercase(config.xinfoType));

                obj.entryVarDict[config.dbName].create.TabPushF("this._{0}Proxy{1} = new {2}Proxy{1}().Init(this);\n",
                    CodeGen.Program.FirstCharacterToLowercase(config.xinfoType),
                    config.postfix,
                    config.xinfoType);
            }
        }
    }
}