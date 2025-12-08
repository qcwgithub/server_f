
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnSocketClose<S> : Handler<S, MsgSocketClose, ResSocketClose>
        where S : Service
    {
        public override MsgType msgType => MsgType._OnSocketClose;

        void LogServerDisconnect(ProtocolClientData socket)
        {
            var self = this.service.data.serviceTypeAndId;
            var remote = socket.serviceTypeAndId.Value;
            if (this.service.data.state < ServiceState.ShuttingDown &&
                !socket.remoteWillShutdown &&
                socket.closeReason != ProtocolClientData.CloseReason.OnConnectComplete_false &&
                self.serviceType.ShouldLogErrorWhenDisconnectFrom(remote.serviceType))
            {
                this.service.logger.FatalFormat("SocketClose {0} reason {1}", remote, socket.closeReason);
            }
            else
            {
                this.service.logger.InfoFormat("SocketClose {0} reason {1}", remote, socket.closeReason);
            }
        }

        public override async Task<ECode> Handle(ProtocolClientData socket, MsgSocketClose msg, ResSocketClose res)
        {
            if (socket.serviceTypeAndId != null)
            {
                this.LogServerDisconnect(socket);
            }

            var sd = this.service.data;
            if (sd.state < ServiceState.ShuttingDown &&
                sd.markedShutdown &&
                sd.GetPassivelyConnections().Count == 0 &&
                !sd.timer_shutdown.IsAlive())
            {
                sd.timer_shutdown = this.server.timerScript.SetTimer(this.service.serviceId, 0, MsgType._Shutdown, new MsgShutdown { force = false });
                this.service.logger.Info("0 passive connections, shutdown in 0 second...");
            }

            return ECode.Success;
        }
    }
}