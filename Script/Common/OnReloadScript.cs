using System.IO;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnReloadScript<S> : Handler<S, MsgReloadScript, ResReloadScript>
        where S : Service
    {
        public OnReloadScript(Server server, S service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._ReloadScript;
        public override async Task<ECode> Handle(IConnection connection, MsgReloadScript msg, ResReloadScript res)
        {
            // if (this.server.data.state != ServerState.Started)
            // {
            //     this.server.logger.Info("server state is " + this.server.data.state + ", can not reload script");
            //     return ECode.Error.toTask();
            // }

            this.service.logger.InfoFormat("{0} local {1}", this.msgType, msg.local);

            // if (this.scriptEntry.GetScriptDllVersion() == msg.version)
            // {
            //     res.message = "success, version not change";
            //     this.service.logger.Info(res.message);
            //     return ECode.Success;
            // }

            string preVersion = this.server.GetScriptDllVersion().ToString();

            // this.service.logger.Info("write files...");
            bool success = true;

            if (!msg.local)
            {
                success = Data.Program.WriteScriptDllAndPdbFile(msg.dllBytes, msg.pdbBytes);
                if (!success)
                {
                    res.message = "write file failed";
                    this.service.logger.Error(res.message);
                    return ECode.Error;
                }
            }

            success = Data.Program.LoadScriptDll(null);
            if (!success)
            {
                res.message = "load dll failed";
                this.service.logger.Error(res.message);
                return ECode.Error;
            }

            res.message = "success";
            this.service.logger.Info(res.message);

            string newVersion = Data.Program.s_assemblyLoadContextRefs[Data.Program.s_assemblyLoadContextRefs.Count - 1].iserver.GetScriptDllVersion().ToString();

            string message = $"{this.service.data.serviceConfig.Tai()} reload script success, {preVersion} -> {newVersion}";
            this.server.feiShuMessenger.SendEventMessage(message);
            this.service.logger.Info(message);

            return ECode.Success;
        }
    }
}