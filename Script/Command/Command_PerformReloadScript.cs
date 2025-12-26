using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Command_PerformReloadScript : Handler<CommandService, MsgCommon, ResCommon>
    {
        public Command_PerformReloadScript(Server server, CommandService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Command_PerformReloadScript;

        public override async Task<ECode> Handle(IConnection connection, MsgCommon msg, ResCommon res)
        {
            int serviceId = (int)msg.GetLong("serviceId");
            string zip = msg.GetString("zip");

            var msgReload = new MsgReloadScript();

            if (string.IsNullOrEmpty(zip))
            {
                msgReload.local = true;
                msgReload.dllBytes = null;
                msgReload.pdbBytes = null;
            }
            else
            {
                byte[]? dllBytes = null;
                byte[]? pdbBytes = null;
                ZipUtils.UnzipInMemory(zip,
                path =>
                {
                    // System.Console.WriteLine(path);
                    int index = path.IndexOf('/');
                    if (index < 0)
                    {
                        return false;
                    }

                    string name = path.Substring(index + 1);
                    return name == "Script.dll" || name == "Script.pdb";
                },
                (path, buffer) =>
                {
                    if (path.Contains("Script.dll"))
                    {
                        dllBytes = buffer;
                    }
                    else
                    {
                        pdbBytes = buffer;
                    }
                });

                if (dllBytes == null)
                {
                    this.service.logger.Error("dllBytes==null");
                    return ECode.Error;
                }

                if (pdbBytes == null)
                {
                    this.service.logger.Error("pdbBytes==null");
                    return ECode.Error;
                }

                msgReload.local = false;
                msgReload.dllBytes = dllBytes;
                msgReload.pdbBytes = pdbBytes;
            }

            var r = await this.service.commandConnectToOtherService.Request<MsgReloadScript, ResReloadScript>(serviceId, MsgType._ReloadScript, msgReload);
            if (r.e != ECode.Success)
            {
                this.service.logger.ErrorFormat("reload script failed, message: {0}", r.res.message);
                return r.e;
            }

            this.service.logger.InfoFormat("reload script ok, message: {0}", r.res.message);
            return r.e;
        }
    }
}