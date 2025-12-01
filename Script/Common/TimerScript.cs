using Data;

namespace Script
{
    // 存放在内存的定时器，精度：秒
    public class TimerScript : ServerScript
    {
        public TimerSData timerSData => this.server.data.timerSData;
        
        public Data.ITimer SetTimer(int serviceId, int timeoutS, MsgType msgType, object msg)
        {
            return this.timerSData.SetTimer(serviceId, timeoutS, msgType, msg, false);
        }

        public Data.ITimer SetLoopTimer(int serviceId, int timeoutS, MsgType msgType, object msg)
        {
            return this.timerSData.SetTimer(serviceId, timeoutS, msgType, msg, true);
        }

        public void ClearTimer(Data.ITimer timer)
        {
            this.timerSData.ClearTimer(timer);
        }
    }
}