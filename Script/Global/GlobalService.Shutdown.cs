namespace Script
{
    public partial class GlobalService
    {
        protected override async Task StopBusinesses()
        {
            s_ClearTimer(this.server, ref this.sd.timer_tick_Loop);
        }
    }
}