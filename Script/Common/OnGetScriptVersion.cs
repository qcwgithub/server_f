using System.IO;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnGetScriptVersion<S> : Handler<S, MsgGetScriptVersion>
        where S : Service
    {
        public override MsgType msgType => MsgType._GetScriptVersion;
        public override Task<MyResponse> Handle(ProtocolClientData socket, MsgGetScriptVersion msg)
        {
            // if (this.server.data.state != ServerState.Started)
            // {
            //     this.server.logger.Info("server state is " + this.server.data.state + ", can not reload script");
            //     return ECode.Error.toTask();
            // }

            this.service.logger.InfoFormat("{0} ", this.msgType);

            var res = new ResGetScriptVersion();
            res.version = this.server.GetScriptDllVersion().ToString();
            return new MyResponse(ECode.Success, res).ToTask();
        }
    }
}