using Data;

namespace Script
{
    public partial class UserService
    {
        public override async Task OnTimer(TimerType timerType, object data)
        {
            switch (timerType)
            {
                case TimerType.SaveUser:
                    break;

                default:
                    await base.OnTimer(timerType, data);
                    break;
            }
        }
    }
}