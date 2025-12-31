using Data;

namespace Script
{
    public partial class Service
    {
        #region auto

        public async Task<MyResponse<ResStart>> Start(MsgStart msg)
        {
            return await this.dispatcher.Dispatch<MsgStart, ResStart>(default, MsgType._Service_Start, msg);
        }
        public async Task<MyResponse<ResShutdown>> Shutdown(MsgShutdown msg)
        {
            return await this.dispatcher.Dispatch<MsgShutdown, ResShutdown>(default, MsgType._Service_Shutdown, msg);
        }
        public async Task<MyResponse<ResCheckConnections>> CheckConnections(MsgCheckConnections msg)
        {
            return await this.dispatcher.Dispatch<MsgCheckConnections, ResCheckConnections>(default, MsgType._Service_CheckConnections, msg);
        }
        public async Task<MyResponse<ResPersistence>> PersistenceTaskQueueHandler(MsgPersistence msg)
        {
            return await this.dispatcher.Dispatch<MsgPersistence, ResPersistence>(default, MsgType._Service_PersistenceTaskQueueHandler, msg);
        }

        #endregion auto
    }
}
