using System;
using System.IO;
using System.Linq;

public class Create_SaveXXX
{
    public static void Create(ServerDataConfig config)
    {
        string dir = string.Format("Script/{0}/", config.dbFilesConfig.scriptFolder);
        Directory.CreateDirectory(dir);

        foreach (var save in config.save)
        {
            string content = Create1(config, save);
            string path = string.Format("{0}Save_{1}{2}.cs", dir, config.profileType, config.postfix);
            File.WriteAllText(path, content);
        }
    }

    static string Create1(ServerDataConfig config, ServerDataConfig.Save save)
    {
        FileFormatter ff = new FileFormatter();
        // ff.TabPush("// Auto created, DO NOT modify it manually\n\n");

        ff.TabPush("using Data;\n");
        ff.TabPush("using System.Threading.Tasks;\n");
        ff.Push("\n");

        config.PushUsingOriginal(ff);

        ff.TabPush("namespace Script\n");
        ff.BlockStart();
        {
            ff.TabPush("//// AUTO CREATED ////\n");
            ff.TabPushF("public sealed class Save_{0}{1} : Handler<{2}, {3}>\n", config.profileType, config.postfix, config.dbFilesConfig.server_class, config.dbFilesConfig.serviceClassName);
            ff.BlockStart();
            {
                ff.TabPushF("public override MsgType msgType => MsgType._Save_{0}{1};\n", config.profileType, config.postfix);
                ff.Push("\n");

                ff.TabPushF("public override async Task<MyResponse> Handle(IConnection connection, object _msg)\n");
                ff.BlockStart();
                {
                    ff.TabPushF("var msg = Utils.CastObject<MsgSave_{0}>(_msg);\n", config.profileType);
                    if (save.field == null)
                        ff.TabPushF("this.service.logger.InfoFormat(\"{{0}}\", this.msgType);\n");
                    else if (save.field2 == null)
                        ff.TabPushF("this.service.logger.InfoFormat(\"{{0}} {0} {{1}}\", this.msgType, msg.info.{0});\n", save.field.name);
                    else
                        ff.TabPushF("this.service.logger.InfoFormat(\"{{0}} {0} {{1}} {1} {{2}}\", this.msgType, msg.info.{0}, msg.info.{1});\n", save.field.name, save.field2.name);
                    ff.Push("\n");

                    ff.TabPushF("ECode e = await this.service.{0}{1}.{2}(msg.info);\n", config.fileName, config.postfix, save.methodName);
                    ff.TabPush("if (e != ECode.Success)\n");
                    ff.BlockStart();
                    {
                        ff.TabPush("return e;\n");
                    }
                    ff.BlockEnd();
                    ff.Push("\n");
                    ff.TabPushF("var res = new ResSave_{0}();\n", config.profileType);
                    ff.TabPush("return new MyResponse(ECode.Success, res);\n");
                }
                ff.BlockEnd();
            }
            ff.BlockEnd();
        }
        ff.BlockEnd();

        return ff.GetString();
    }
}