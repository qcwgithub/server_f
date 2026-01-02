using System.Linq;
using System.Collections.Generic;
using Data;

namespace Script
{
    public partial class CommandService
    {
        public async Task<ECode> PerformReloadScript(MsgCommon msg)
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
                    this.logger.Error("dllBytes==null");
                    return ECode.Error;
                }

                if (pdbBytes == null)
                {
                    this.logger.Error("pdbBytes==null");
                    return ECode.Error;
                }

                msgReload.local = false;
                msgReload.dllBytes = dllBytes;
                msgReload.pdbBytes = pdbBytes;
            }

            var r = await this.commandConnectToOtherService.Request(serviceId, MsgType._Service_ReloadScript, msgReload);
            var resLoad = r.CastRes<ResReloadScript>();
            if (r.e != ECode.Success)
            {
                this.logger.ErrorFormat("reload script failed, message: {0}", resLoad.message);
                return r.e;
            }

            this.logger.InfoFormat("reload script ok, message: {0}", resLoad.message);
            return r.e;
        }
    }
}