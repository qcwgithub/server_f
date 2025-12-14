using System;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic;

public class Create_SaveXXX
{
    public static void Create(ServerDataConfig config)
    {
        string dir = string.Format("Script/{0}/", config.dbFilesConfig.scriptFolder);
        Directory.CreateDirectory(dir);

        foreach (var save in config.save)
        {
            string content = Create1(config, save);
            string path = string.Format("{0}Save_{1}{2}.cs", dir, config.xinfoType, config.postfix);
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
            ff.TabPushF("public sealed class Save_{0}{1} : Handler<{2}, MsgSave_{0}, ResSave_{0}>\n", config.xinfoType, config.postfix, config.dbFilesConfig.serviceClassName);
            ff.BlockStart();
            {
                ff.TabPushF("public override MsgType msgType => MsgType._Save_{0}{1};\n", config.xinfoType, config.postfix);
                ff.Push("\n");
                
                ff.TabPushF("public Save_{0}{1}(Server server, {2} service) : base(server, service)\n", config.xinfoType, config.postfix, config.dbFilesConfig.serviceClassName);
                ff.BlockStart();
                ff.BlockEnd();
                ff.Push("\n");

                ff.TabPushF("public override async Task<ECode> Handle(IConnection connection, MsgSave_{0} msg, ResSave_{0} res)\n", config.xinfoType);
                ff.BlockStart();
                {
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
                    ff.TabPush("return ECode.Success;\n");
                }
                ff.BlockEnd();
            }
            ff.BlockEnd();
        }
        ff.BlockEnd();

        return ff.GetString();
    }
}