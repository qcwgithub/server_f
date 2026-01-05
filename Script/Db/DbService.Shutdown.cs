using Data;

namespace Script
{
    public partial class DbService
    {
        protected override async Task StopBusinesses()
        {
            if (this.sd.timer_persistence_taskQueueHandler_Loop != null)
            {
                this.ClearTimer(ref this.sd.timer_persistence_taskQueueHandler_Loop);
            }

            while (this.sd.persistenceHandling)
            {
                await Task.Delay(10);
            }

            ECode e = await this.Persistence(true);
            if (e != ECode.Success)
            {
                this.logger.Error($"Persistence ECode.{e}");
            }
        }
    }
}