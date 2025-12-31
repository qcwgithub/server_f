
using Data;

namespace Script
{
    public class OnConnectionClose<S> : Handler<S, MsgOnConnectionClose, ResOnConnectionClose>
        where S : Service
    {
        public OnConnectionClose(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Service_OnConnectionClose;

        public override async Task<ECode> Handle(MessageContext context, MsgOnConnectionClose msg, ResOnConnectionClose res)
        {
            if (context.connection is ServiceConnection serviceConnection)
            {
                var sd = this.service.data;

                if (sd.state < ServiceState.ShuttingDown &&
                    !serviceConnection.remoteWillShutdown &&
                    serviceConnection.closeReason != ProtocolClientData.CloseReason.OnConnectComplete_false &&
                    sd.serviceType.ShouldLogErrorWhenDisconnectFrom(serviceConnection.serviceType))
                {
                    this.service.logger.FatalFormat("SocketClose {0} reason {1}", serviceConnection.tai, serviceConnection.closeReason);
                }
                else
                {
                    this.service.logger.InfoFormat("SocketClose {0} reason {1}", serviceConnection.tai, serviceConnection.closeReason);
                }

                if (sd.state < ServiceState.ShuttingDown &&
                    sd.markedShutdown &&
                    sd.GetPassivelyConnections().Count == 0 &&
                    !sd.timer_shutdown.IsAlive())
                {
                    sd.timer_shutdown = this.server.timerScript.SetTimer(this.service.serviceId, 0, MsgType._Service_Shutdown, new MsgShutdown { force = false });
                    this.service.logger.Info("0 passive connections, shutdown in 0 second...");
                }
            }

            return ECode.Success;
        }
    }
}