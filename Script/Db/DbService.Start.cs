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
            await this.collection_scene_info.CreateIndex();
            await this.collection_user_brief_info.CreateIndex();
            await this.collection_friend_chat_info.CreateIndex();
            await this.collection_friend_chat_message.CreateIndex();

            #endregion auto_callCreateIndex

            await this.collection_user_info.CreateIndex();

            sd.timer_persistence_taskQueueHandler_Loop = this.server.timerScript.SetTimer(this.serviceId, 0,  TimerType.Persistence, null);
            return ECode.Success;
        }
    }
}