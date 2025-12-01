using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnGetServiceState<S> : Handler<S>
        where S : Service
    {
        public override MsgType msgType => MsgType._GetServiceState;

        public override Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var res = new ResGetServiceState();
            res.serviceType = this.service.data.serviceType;
            res.serviceId = this.service.data.serviceId;
            res.serviceState = this.service.data.state;
            return new MyResponse(ECode.Success, res).ToTask();
        }
    }
}