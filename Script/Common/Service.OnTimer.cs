using Data;

namespace Script
{
    public partial class Service
    {
        public virtual async Task OnTimer(TimerType timerType, object data)
        {
            switch (timerType)
            {
                default:
                    throw new Exception("Not handled TimerType." + timerType);
            }
        }
    }
}