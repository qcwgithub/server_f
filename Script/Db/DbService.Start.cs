using Data;

namespace Script
{
    public partial class DbService
    {
        protected override async Task<ECode> Start2()
        {
            #region auto_callCreateIndex

            await this.collection_user_info.CreateIndex();
            await this.collection_account_info.CreateIndex();
            await this.collection_room_info.CreateIndex();

            #endregion auto_callCreateIndex

            this.logger.Error("NEED RESTORE HERE");
            // await this.service.collection_user_info.CreateIndex();

            sd.timer_persistence_taskQueueHandler_Loop = this.server.timerScript.SetTimer(this.serviceId, 0,  TimerType.PersistenceTaskQueueHandler_Loop, null);
            return ECode.Success;
        }
    }
}