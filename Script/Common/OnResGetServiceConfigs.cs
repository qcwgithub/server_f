using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnResGetServiceConfigs<S> : Handler<S, A_ResGetServiceConfigs>
        where S : Service
    {
        public override MsgType msgType => MsgType._A_ResGetServiceConfigs;

        public override Task<MyResponse> Handle(ProtocolClientData socket, A_ResGetServiceConfigs msg)
        {
            this.service.logger.InfoFormat("{0}", this.msgType);
            this.service.data.SaveServiceConfigs(msg.res);
            return ECode.Success.ToTask();
        }
    }
}