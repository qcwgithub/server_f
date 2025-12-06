using System.Threading.Tasks;
using Data;
using System.Collections.Generic;

namespace Script
{
    public class OnGetReloadConfigOptions<S> : Handler<S, MsgGetReloadConfigOptions>
        where S : Service
    {
        public override MsgType msgType => MsgType._GetReloadConfigOptions;

        public override Task<MyResponse> Handle(ProtocolClientData socket, MsgGetReloadConfigOptions msg)
        {
            this.service.logger.InfoFormat("{0}", this.msgType);

            var res = new ResGetReloadConfigOptions();
            res.files = new List<string>();

            this.service.data.GetReloadConfigOptions(res.files);

            return new MyResponse(ECode.Success, res).ToTask();
        }
    }
}