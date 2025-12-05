using Data;
using System.Threading.Tasks;

namespace Script
{
    public class Db_Shutdown : OnShutdown<DbService>
    {
        protected override async Task StopBusinesses()
        {
            this.ClearTimer(ref this.service.databaseServiceData.timer_persistence_taskQueueHandler_Loop);

            DbServiceData sd = this.service.databaseServiceData;
            while (sd.persistenceHandling)
            {
                await Task.Delay(10);
            }

            var msgPersistence = new MsgPersistence();
            msgPersistence.isShuttingDownSaveAll = true;
            MyResponse r = await this.service.connectToSelf.SendToSelfAsync(MsgType._PersistenceTaskQueueHandler, msgPersistence);
            if (r.err != ECode.Success)
            {
                this.service.logger.ErrorFormat("{0} save all r.err {1}", this.msgType, r.err);
            }
        }
    }
}