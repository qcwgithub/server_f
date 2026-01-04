using Data;

namespace Script
{
    public partial class DbService
    {
        protected override async Task StopBusinesses()
        {
            this.ClearTimer(ref this.sd.timer_persistence_taskQueueHandler_Loop);

            while (this.sd.persistenceHandling)
            {
                await Task.Delay(10);
            }

            var msgPersistence = new MsgPersistence();
            msgPersistence.isShuttingDownSaveAll = true;
            // ECode e = await this.PersistenceTaskQueueHandler(msgPersistence);
            // if (e != ECode.Success)
            // {
            //     this.logger.ErrorFormat("StopBusinesses save all r.err {0}", e);
            // }
            throw new Exception("TODO");
        }
    }
}