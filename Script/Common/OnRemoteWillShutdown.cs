using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnRemoteWillShutdown<S> : Handler<S, MsgRemoteWillShutdown, ResRemoteWillShutdown>
        where S : Service
    {
        public override MsgType msgType => MsgType._RemoteWillShutdown;

        public override async Task<ECode> Handle(ProtocolClientData socket, MsgRemoteWillShutdown msg, ResRemoteWillShutdown res)
        {
            this.service.logger.InfoFormat("{0} {1}", this.msgType, socket.serviceTypeAndId == null ? "null" : socket.serviceTypeAndId.Value.ToString());
            socket.remoteWillShutdown = true;
            return ECode.Success;
        }
    }
}