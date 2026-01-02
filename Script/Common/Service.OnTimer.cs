using Data;

namespace Script
{
    public partial class Service
    {
        public virtual async Task OnTimer(TimerType timerType, object data)
        {
            ECode e;
            switch (timerType)
            {
                case TimerType.CheckConnections:
                    e = await this.CheckConnections();

                    if (this.data.state < ServiceState.ShuttingDown)
                    {
                        this.data.timer_CheckConnections = this.server.timerScript.SetTimer(this.serviceId, this.data.state == ServiceState.Started ? 31 : 1, TimerType.CheckConnections, null);
                    }
                    break;

                default:
                    throw new Exception("Not handled TimerType." + timerType);
            }
        }
    }
}