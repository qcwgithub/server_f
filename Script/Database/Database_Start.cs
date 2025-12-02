using Data;

namespace Script
{
    public partial class Database_Start : OnStart<DatabaseService>
    {
        protected override async Task<ECode> Handle2()
        {
            #region auto_callCreateIndex

            #endregion auto_callCreateIndex

            await this.service.collection_player.CreateIndex();

            await s_ObtainInit("Database.Start", this.service.lockController, new string[] { DatabaseKey.LockKey.DBPlayerInit() }, async () =>
            {
                ProfileDBPlayer profileDBPlayer = await this.service.collection_profile_dbplayer.Query_ProfileDBPlayer_all();

                bool save = false;
                if (profileDBPlayer == null)
                {
                    this.service.logger.Info("Create ProfileDBPlayer");
                    this.server.feiShuMessenger.SendEventMessage("Create ProfileDBPlayer");

                    profileDBPlayer = new ProfileDBPlayer();
                    save = true;
                }
                profileDBPlayer.Ensure();

                if (save)
                {
                    await this.service.collection_profile_dbplayer.Save(profileDBPlayer);
                }
            },
            this.service.logger);

            var sd = this.service.databaseServiceData;
            sd.timer_persistence_taskQueueHandler_Loop = this.server.timerScript.SetTimer(this.service.serviceId, 0, MsgType._PersistenceTaskQueueHandler_Loop, null);
            return ECode.Success;
        }
    }
}