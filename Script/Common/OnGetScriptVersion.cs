using System.IO;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnGetScriptVersion<S> : Handler<S, MsgGetScriptVersion, ResGetScriptVersion>
        where S : Service
    {
        public OnGetScriptVersion(Server server, S service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._GetScriptVersion;
        public override async Task<ECode> Handle(MsgContext context, MsgGetScriptVersion msg, ResGetScriptVersion res)
        {
            // if (this.server.data.state != ServerState.Started)
            // {
            //     this.server.logger.Info("server state is " + this.server.data.state + ", can not reload script");
            //     return ECode.Error.toTask();
            // }

            this.service.logger.InfoFormat("{0} ", this.msgType);

            res.version = this.server.GetScriptDllVersion().ToString();
            return ECode.Success;
        }
    }
}