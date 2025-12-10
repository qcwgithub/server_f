using Data;
using System.Threading.Tasks;

namespace Script
{
    public class Db_Shutdown : OnShutdown<DbService>
    {
        public Db_Shutdown(Server server, DbService service) : base(server, service)
        {
        }

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
            var r = await this.service.connectToSelf.Request<MsgPersistence, ResPersistence>(MsgType._PersistenceTaskQueueHandler, msgPersistence);
            if (r.e != ECode.Success)
            {
                this.service.logger.ErrorFormat("{0} save all r.err {1}", this.msgType, r.e);
            }
        }
    }
}