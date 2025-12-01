using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnRemoteWillShutdown<S> : Handler<S>
        where S : Service
    {
        public override MsgType msgType => MsgType._RemoteWillShutdown;

        public override Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            this.service.logger.InfoFormat("{0} {1}", this.msgType, socket.serviceTypeAndId == null ? "null" : socket.serviceTypeAndId.Value.ToString());
            socket.remoteWillShutdown = true;
            return ECode.Success.ToTask();
        }
    }
}