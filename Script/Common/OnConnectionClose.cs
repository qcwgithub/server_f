
using Data;

namespace Script
{
    public partial class Service
    {
        public virtual async Task<ECode> OnConnectionClose(IConnection connection)
        {
            if (connection is ServiceConnection serviceConnection)
            {
                if (this.data.state < ServiceState.ShuttingDown &&
                    !serviceConnection.remoteWillShutdown &&
                    serviceConnection.closeReason != ProtocolClientData.CloseReason.OnConnectComplete_false &&
                    this.data.serviceType.ShouldLogErrorWhenDisconnectFrom(serviceConnection.serviceType))
                {
                    this.logger.FatalFormat("SocketClose {0} reason {1}", serviceConnection.tai, serviceConnection.closeReason);
                }
                else
                {
                    this.logger.InfoFormat("SocketClose {0} reason {1}", serviceConnection.tai, serviceConnection.closeReason);
                }

                if (this.data.state < ServiceState.ShuttingDown &&
                    this.data.markedShutdown &&
                    this.data.GetPassivelyConnections().Count == 0 &&
                    !this.data.timer_shutdown.IsAlive())
                {
                    this.data.timer_shutdown = this.server.timerScript.SetTimer(this.serviceId, 0, MsgType._Service_Shutdown, new MsgShutdown { force = false });
                    this.logger.Info("0 passive connections, shutdown in 0 second...");
                }
            }

            return ECode.Success;
        }
    }
}