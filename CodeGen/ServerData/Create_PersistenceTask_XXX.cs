using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
public class Create_PersistenceTask_XXX
{
    public static void Create(List<ServerDataConfig> list)
    {
        // dirty element
        var f_create = new FileFormatter();
        f_create.AddTab(2);

        var f_toString = new FileFormatter();
        f_toString.AddTab(4);

        var f_fromString = new FileFormatter();
        f_fromString.AddTab(4);

        // enum
        var f_enum = new FileFormatter();
        f_enum.AddTab(2);

        // callOnSave
        var f_callSaveDict = new Dictionary<string, FileFormatter>();
        foreach (string dbName in ServerDataConfig.c_dbNames)
        {
            f_callSaveDict[dbName] = new FileFormatter();
            f_callSaveDict[dbName].AddTab(4);
        }

        foreach (var config in list)
        {
            if (!config.createPersistence)
            {
                continue;
            }

            Create_PersistenceTask_XXX_cs(config);

            Create_f_create(config, f_create);
            Create_f_toString(config, f_toString);
            Create_f_fromString(config, f_fromString);

            f_enum.TabPush(config.profileType, config.postfix, ",\n");

            Create_f_callSave(config, f_callSaveDict);
        }


        ProfileProgram.ReplaceFile("server/Data/Common/stDirtyElement.cs", new Mark[]
        {
                new Mark { startMark = "#region auto_create", text = f_create.GetString() },
                new Mark { startMark = "#region auto_toString", text = f_toString.GetString() },
                new Mark { startMark = "#region auto_fromString", text = f_fromString.GetString() },
        });

        ProfileProgram.ReplaceFile("server/Data/Common/DirtyElementType.cs", new Mark[]
        {
            new Mark { startMark = "#region auto_enum", text = f_enum.GetString() },
        });

        foreach (string dbName in ServerDataConfig.c_dbNames)
        {
            ProfileProgram.ReplaceFile(ServerDataConfig.s_dbFilesConfigDict[dbName].PersistenceTaskQueueHandler_path, new Mark[]
            {
                new Mark { startMark = "#region auto_callSave", text = f_callSaveDict[dbName].GetString() },
            });
        }
    }

    static void Create_f_create(ServerDataConfig config, FileFormatter f_create)
    {
        f_create.TabPushF("public static stDirtyElement Create_{0}{1}({2})\n", config.profileType, config.postfix, config.keyParamToString(true, true, string.Empty, false, false, false));
        f_create.BlockStart();
        {
            f_create.TabPushF("return new stDirtyElement {{ e = DirtyElementType.{0}{1}, ", config.profileType, config.postfix);

            for (int k = 0; k < config.keyParam.Count; k++)
            {
                switch (config.keyParam[k].type)
                {
                    case "int":
                    case "long":
                    case "longid":
                        f_create.Push(string.Format("s{0} = {1}.ToString()", k + 1, config.keyParam[k].name));
                        break;

                    case "string":
                        f_create.Push(string.Format("s{0} = {1}", k + 1, config.keyParam[k].name));
                        break;

                    default:
                        if (config.keyParam[k].typeModifier == "enum")
                        {
                            f_create.Push(string.Format("s{0} = {1}.ToString()", k + 1, config.keyParam[k].name));
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        break;
                }

                if (k < config.keyParam.Count - 1)
                {
                    f_create.Push(", ");
                }
            }

            f_create.Push(" };\n");
        }
        f_create.BlockEnd();
    }

    static void Create_f_toString(ServerDataConfig config, FileFormatter f_toString)
    {
        f_toString.TabPushF("case DirtyElementType.{0}{1}:\n", config.profileType, config.postfix);
        f_toString.AddTab(1);
        {
            f_toString.TabPushF("return string.Join(SPLITER, this.e");

            for (int k = 0; k < config.keyParam.Count; k++)
            {
                f_toString.Push(string.Format(", this.s{0}", k + 1));
            }

            f_toString.Push(");\n");
            f_toString.Push("\n");
        }
        f_toString.AddTab(-1);
        // f_toString.TabPushF("break;\n");
    }

    static void Create_f_fromString(ServerDataConfig config, FileFormatter f_fromString)
    {
        f_fromString.TabPushF("case DirtyElementType.{0}{1}:\n", config.profileType, config.postfix);
        f_fromString.AddTab(1);
        {
            int L = config.keyParam.Count;
            if (L == 1)
            {
                f_fromString.TabPush("self.s1 = str.Substring(index + 1);\n");
            }
            else if (L == 2)
            {
                f_fromString.BlockStart();
                f_fromString.TabPushF("int index2 = str.IndexOf(SPLITER, index + 1);\n");
                f_fromString.TabPush("self.s1 = str.Substring(index + 1, index2 - index - 1);\n");
                f_fromString.TabPush("self.s2 = str.Substring(index2 + 1);\n");
                f_fromString.BlockEnd();
            }
        }
        f_fromString.TabPushF("break;\n\n");
        f_fromString.AddTab(-1);
    }

    static void Create_f_callSave(ServerDataConfig config, Dictionary<string, FileFormatter> f_callSaveDict)
    {
        var f_callSave = f_callSaveDict[config.dbName];

        f_callSave.TabPushF("case DirtyElementType.{0}{1}:\n", config.profileType, config.postfix);
        f_callSave.AddTab(1);
        {
            f_callSave.TabPushF("(err, putBack) = await this.Save{0}{1}(element);\n", config.profileType, config.postfix);
        }
        f_callSave.TabPushF("break;\n\n");
        f_callSave.AddTab(-1);
    }

    static void Create_PersistenceTask_XXX_cs(ServerDataConfig config)
    {
        var ff = new FileFormatter();

        ff.TabPush("using System.Threading.Tasks;\n");
        ff.TabPush("using Data;\n");
        ff.TabPush("using System.Diagnostics;\n");
        ff.TabPush("using System;\n");
        ff.TabPush("using longid = System.Int64;\n");

        ff.Push("\n");
        config.PushUsingOriginal(ff);

        ff.Push("\n");
        ff.TabPush("namespace Script\n");
        ff.BlockStart();
        {
            ff.TabPushF("public partial class {0}\n", ServerDataConfig.s_dbFilesConfigDict[config.dbName].PersistenceTaskQueueHandler_class);
            ff.BlockStart();
            {
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("async Task<(ECode, bool)> Save{0}{1}(stDirtyElement element)\n", config.profileType, config.postfix);
                ff.BlockStart();
                {
                    int _id = 0;
                    foreach (var field in config.keyParam)
                    {
                        _id++;
                        switch (field.type)
                        {
                            case "int":
                                ff.TabPushF("int {0} = int.Parse(element.s{1});\n", field.name, _id);
                                if (field.typeModifier == "gte0")
                                    ff.TabPushF("MyDebug.Assert({0} >= 0);\n", field.name);
                                else
                                    ff.TabPushF("MyDebug.Assert({0} > 0);\n", field.name);
                                break;

                            case "long":
                                ff.TabPushF("long {0} = long.Parse(element.s{1});\n", field.name, _id);
                                ff.TabPushF("MyDebug.Assert({0} > 0);\n", field.name);
                                break;

                            case "longid":
                                ff.TabPushF("longid {0} = longid.Parse(element.s{1});\n", field.name, _id);
                                ff.TabPushF("MyDebug.Assert({0} > 0);\n", field.name);
                                break;

                            case "string":
                                ff.TabPushF("string {0} = element.s{1};\n", field.name, _id);
                                ff.TabPushF("MyDebug.Assert(!string.IsNullOrEmpty({0}));\n", field.name);
                                break;

                            default:
                                if (field.typeModifier == "enum")
                                {
                                    ff.TabPushF("{0} {1} = Enum.Parse<{0}>(element.s{2});\n", field.type, field.name, _id);
                                    ff.TabPushF("MyDebug.Assert({0}.IsEnumValueValid());\n", field.name);
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }
                                break;
                        }
                    }

                    ff.Push("\n");

                    if (config.createProxy)
                    {
                        string getProxy;
                        if (config.copy != null)
                        {
                            getProxy = string.Format("{0}Proxy({1}.Id_{2})", CodeGen.Program.FirstCharacterToLowercase(config.profileType), config.copy.config.name, config.copy.index + 1);
                        }
                        else
                        {
                            getProxy = string.Format("{0}Proxy{1}", CodeGen.Program.FirstCharacterToLowercase(config.profileType), config.postfix);
                        }

                        ff.TabPushF("{0} info = await this.server.{1}.OnlyForSave_GetFromRedis({2});\n",
                            config.profileType,
                            getProxy,
                            config.keyParamToString(false, true, string.Empty, false, false, false));
                    }
                    else
                    {
                        ff.TabPushF("{0} info = await this.server.{1}Redis{2}.OnlyForSave_GetFromRedis(this.service, {3});\n",
                            config.profileType,
                            CodeGen.Program.FirstCharacterToLowercase(config.profileType),
                            config.postfix,
                            config.keyParamToString(false, true, string.Empty, false, false, false));
                    }

                    ff.TabPush("if (info == null)\n");
                    ff.BlockStart();
                    {
                        ff.TabPushF("this.service.logger.ErrorFormat(\"Save{0} {{0}} info==null\", element);\n", config.profileType);
                        ff.TabPush("return (ECode.Error, false);\n");
                    }
                    ff.BlockEnd();

                    foreach (var field in config.keyParam)
                    {
                        ff.TabPushF("MyDebug.Assert(info.{0} == {0});\n", field.name);
                    }

                    ff.Push("\n");
                    ff.TabPush("if (info is ICanBePlaceholder h && h.IsPlaceholder())\n");
                    ff.BlockStart();
                    {
                        ff.TabPushF("this.service.logger.ErrorFormat(\"Save{0} {{0}} info.IsPlaceholder()\", element);\n", config.profileType);
                        ff.TabPush("return (ECode.Error, false);\n");
                    }
                    ff.BlockEnd();

                    ff.Push("\n");
                    // SaveToDB1(config, ff);
                    SaveToDBSelf(config, ff);

                    ff.Push("\n");
                    ff.TabPush("return (ECode.Success, false);\n");
                }
                ff.BlockEnd();
            }
            ff.BlockEnd();
        }
        ff.BlockEnd();

        string path = ServerDataConfig.s_dbFilesConfigDict[config.dbName].PersistenceTaskQueueHandler_path2(config.profileType, config.postfix);
        if (!File.Exists(path))
        {
            File.WriteAllText(path, string.Empty);
            // throw new FileNotFoundException(path);
        }
        File.WriteAllText(path, ff.GetString());
    }

    public static void SaveToDBSelf(ServerDataConfig config, FileFormatter ff)
    {
        ff.TabPushF("ECode e = await this.service.{0}{1}.{2}(info);\n", config.fileName, config.postfix, "Save");
        ff.TabPush("if (e != ECode.Success)\n");
        ff.BlockStart();
        {
            ff.TabPushF("this.service.logger.ErrorFormat(\"Save{0} {{0}} error {{1}}\", element, e);\n", config.profileType);
            ff.TabPush("return (e, true);\n");
        }
        ff.BlockEnd();
    }

    public static void SaveToDB1(ServerDataConfig config, FileFormatter ff)
    {
        ff.TabPushF("var msgDb = new MsgSave_{0}{1}();\n", config.profileType, config.postfix);
        ff.TabPush("msgDb.info = info;\n");

        ff.TabPushF("MyResponse r = await this.service.tcpClientScript.SendToServiceAsync(ServiceType.{0}, MsgType._Save_{1}{2}, msgDb);\n", config.dbFilesConfig.serviceType, config.profileType, config.postfix);
        ff.TabPush("if (r.err != ECode.Success)\n");
        ff.BlockStart();
        {
            ff.TabPushF("this.service.logger.ErrorFormat(\"Save{0} {{0}} error {{1}}\", element, r.err);\n", config.profileType);
            ff.TabPush("return (ECode.Error, true);\n");
        }
        ff.BlockEnd();
    }

    public static void SaveToDB2(ServerDataConfig config, FileFormatter ff)
    {
        ff.TabPushF("var msgDb = new MsgSave_{0}{1}();\n", config.profileType, config.postfix);
        ff.TabPush("msgDb.info = info;\n");

        ff.TabPushF("MyResponse r = await this.server.SendToServiceAsync(ServiceType.{0}, MsgType._Save_{1}{2}, msgDb);\n", config.dbFilesConfig.serviceType, config.profileType, config.postfix);
        ff.TabPush("if (r.err != ECode.Success)\n");
        ff.BlockStart();
        {
            if (config.keyParam.Count == 0)
                ff.TabPushF("this.scriptEntry.firstLogger.ErrorFormat(\"Save {0} error {{0}}\", r.err);\n", config.profileType);
            else if (config.keyParam.Count == 1)
                ff.TabPushF("this.scriptEntry.firstLogger.ErrorFormat(\"Save {0} {1} {{0}} error {{1}}\", info.{1}, r.err);\n", config.profileType, config.keyParam[0].name);
            else
                ff.TabPushF("this.scriptEntry.firstLogger.ErrorFormat(\"Save {0} {1} {{0}} {2} {{1}} error {{2}}\", info.{1}, info.{2}, r.err);\n", config.profileType, config.keyParam[0].name, config.keyParam[1].name);
            ff.TabPush("return r.err;\n");
        }
        ff.BlockEnd();
        ff.TabPush("return ECode.Success;\n");
    }
}