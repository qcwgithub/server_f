
using Data;

namespace Script
{
    public class OnConnectionClose<S> : Handler<S, MsgConnectionClose, ResConnectionClose>
        where S : Service
    {
        public OnConnectionClose(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._OnConnectionClose;

        void LogServerDisconnect(ServiceConnection connection)
        {
            var self = this.service.data.serviceTypeAndId;
            var remote = connection.serviceTypeAndId.Value;
            if (this.service.data.state < ServiceState.ShuttingDown &&
                !connection.remoteWillShutdown &&
                connection.closeReason != ProtocolClientData.CloseReason.OnConnectComplete_false &&
                self.serviceType.ShouldLogErrorWhenDisconnectFrom(remote.serviceType))
            {
                this.service.logger.FatalFormat("SocketClose {0} reason {1}", remote, connection.closeReason);
            }
            else
            {
                this.service.logger.InfoFormat("SocketClose {0} reason {1}", remote, connection.closeReason);
            }
        }

        public override async Task<ECode> Handle(IConnection connection, MsgConnectionClose msg, ResConnectionClose res)
        {
            if (connection is ServiceConnection serviceConnection)
            {
                this.LogServerDisconnect(serviceConnection);

                var sd = this.service.data;
                if (sd.state < ServiceState.ShuttingDown &&
                    sd.markedShutdown &&
                    sd.GetPassivelyConnections().Count == 0 &&
                    !sd.timer_shutdown.IsAlive())
                {
                    sd.timer_shutdown = this.server.timerScript.SetTimer(this.service.serviceId, 0, MsgType._Shutdown, new MsgShutdown { force = false });
                    this.service.logger.Info("0 passive connections, shutdown in 0 second...");
                }
            }

            return ECode.Success;
        }
    }
}