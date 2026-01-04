using Data;

namespace Script
{
    // 存放在内存的定时器，精度：秒
    public class TimerScript : ServerScript
    {
        public TimerScript(Server server) : base(server)
        {
            
        }

        public TimerSData timerSData => this.server.data.timerSData;
        
        public Data.ITimer SetTimer(int serviceId, int timeoutS, TimerType timerType, object? data)
        {
            return this.timerSData.SetTimer(serviceId, timeoutS, timerType, data, false);
        }

        public Data.ITimer SetLoopTimer(int serviceId, int timeoutS, TimerType timerType, object? data)
        {
            return this.timerSData.SetTimer(serviceId, timeoutS, timerType, data, true);
        }

        public void ClearTimer(Data.ITimer timer)
        {
            this.timerSData.ClearTimer(timer);
        }
    }
}