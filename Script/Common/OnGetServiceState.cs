using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnGetServiceState<S> : Handler<S, MsgGetServiceState, ResGetServiceState>
        where S : Service
    {
        public override MsgType msgType => MsgType._GetServiceState;

        public override Task<ECode> Handle(ProtocolClientData socket, MsgGetServiceState msg, ResGetServiceState res)
        {
            var res = new ResGetServiceState();
            res.serviceType = this.service.data.serviceType;
            res.serviceId = this.service.data.serviceId;
            res.serviceState = this.service.data.state;
            return ECode.Success;
        }
    }
}