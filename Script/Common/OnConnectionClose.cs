
using Data;

namespace Script
{
    public partial class Service
    {
        public virtual async Task<ECode> OnConnectionClose(IConnection connection)
        {
            if (connection is IServiceConnection serviceConnection)
            {
                if (this.data.state < ServiceState.ShuttingDown &&
                    serviceConnection.knownWho &&
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
                    !this.data.timer_shutdown.IsAlive())
                {
                    var passivelyTais = this.data.GetPassivelyConnections();
                    if (passivelyTais.Count > 0)
                    {
                        this.logger.Info($"{passivelyTais.Count} passive connections: {JsonUtils.stringify(passivelyTais)}");
                    }
                    else
                    {
                        this.data.timer_shutdown = this.server.timerScript.SetTimer(this.serviceId, 0, TimerType.Shutdown, false);
                        this.logger.Info("0 passive connections, shutdown in 0 second...");
                    }
                }
            }

            return ECode.Success;
        }
    }
}