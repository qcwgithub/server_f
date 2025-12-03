using System;
using System.IO;
using System.Linq;

public class Create_QueryXXX
{
    public static void Create(ServerDataConfig config)
    {
        string dir = string.Format("Script/{0}/", config.dbFilesConfig.scriptFolder);
        Directory.CreateDirectory(dir);

        foreach (var query in config.query)
        {
            string content = Create1(config, query);
            string path = string.Format("{0}{1}{2}.cs", dir, query.methodName, config.postfix);
            File.WriteAllText(path, content);
        }
    }

    static string Create1(ServerDataConfig config, ServerDataConfig.Query query)
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
            
            ff.TabPushF("public sealed class {0}{1} : Handler<{2}, {3}>\n", query.methodName, config.postfix, config.dbFilesConfig.server_class, config.dbFilesConfig.serviceClassName);
            ff.BlockStart();
            {
                ff.TabPushF("public override MsgType msgType => MsgType._{0}{1};\n", query.methodName, config.postfix);
                ff.Push("\n");

                ff.TabPushF("public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)\n");
                ff.BlockStart();
                {
                    ff.TabPushF("var msg = Utils.CastObject<Msg{0}>(_msg);\n", query.methodName);

                    if (query.methodParamExps.Length == 0)
                        ff.TabPushF("// this.service.logger.InfoFormat(\"{{0}}\", this.msgType);\n");
                    else
                    {
                        int _id = 0;
                        ff.TabPushF("// this.service.logger.InfoFormat(\"{{0}} {0}\", this.msgType, {1});\n",
                        string.Join(", ", query.methodParamExps.Select(_ => _.Substring(_.LastIndexOf(' ') + 1) + ": {" + (++_id) + "}")),
                        string.Join(", ", query.methodParamExps.Select(_ => "msg." + _.Substring(_.LastIndexOf(' ') + 1))));
                    }
                    ff.Push("\n");

                    ff.TabPushF("var result = await this.service.{0}{1}.{2}({3});\n", config.fileName, config.postfix, query.methodName,
                        string.Join(", ", query.methodParamExps.Select(_ => "msg." + _.Substring(_.LastIndexOf(' ') + 1))));

                    ff.Push("\n");
                    ff.TabPushF("var res = new Res{0}();\n", query.methodName);
                    ff.TabPushF("res.result = result;\n");
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