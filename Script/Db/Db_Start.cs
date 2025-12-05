using Data;

namespace Script
{
    public partial class Db_Start : OnStart<DbService>
    {
        protected override async Task<ECode> Handle2()
        {
            #region auto_callCreateIndex

            #endregion auto_callCreateIndex

            await this.service.collection_user_profile.CreateIndex();

            var sd = this.service.databaseServiceData;
            sd.timer_persistence_taskQueueHandler_Loop = this.server.timerScript.SetTimer(this.service.serviceId, 0, MsgType._PersistenceTaskQueueHandler_Loop, null);
            return ECode.Success;
        }
    }
}