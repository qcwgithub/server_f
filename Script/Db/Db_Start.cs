using Data;

namespace Script
{
    public partial class Db_Start : OnStart<DbService>
    {
        public Db_Start(Server server, DbService service) : base(server, service)
        {
        }


        protected override async Task<ECode> Handle2()
        {
            #region auto_callCreateIndex

            await this.service.collection_user_info.CreateIndex();
            await this.service.collection_account_info.CreateIndex();
            await this.service.collection_room_info.CreateIndex();

            #endregion auto_callCreateIndex

            this.service.logger.Error("NEED RESTORE HERE");
            // await this.service.collection_user_info.CreateIndex();

            var sd = this.service.sd;
            sd.timer_persistence_taskQueueHandler_Loop = this.server.timerScript.SetTimer(this.service.serviceId, 0, MsgType._Service_PersistenceTaskQueueHandler_Loop, null);
            return ECode.Success;
        }
    }
}