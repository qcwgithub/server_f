using Data;

namespace Script
{
    public partial class Service
    {
        #region auto

        public async Task<MyResponse> Start(MsgStart msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Service_Start, msg);
        }
        public async Task<MyResponse> Shutdown(MsgShutdown msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Service_Shutdown, msg);
        }
        public async Task<MyResponse> OnConnectComplete(MsgOnConnectComplete msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Service_OnConnectComplete, msg);
        }
        public async Task<MyResponse> OnConnectionClose(MsgOnConnectionClose msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Service_OnConnectionClose, msg);
        }
        public async Task<MyResponse> CheckConnections(MsgCheckConnections msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Service_CheckConnections, msg);
        }
        public async Task<MyResponse> OnHttpRequest(MsgOnHttpRequest msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Service_OnHttpRequest, msg);
        }
        public async Task<MyResponse> WaitTask(MsgWaitTask msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Service_WaitTask, msg);
        }
        public async Task<MyResponse> PersistenceTaskQueueHandler(MsgPersistence msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Service_PersistenceTaskQueueHandler, msg);
        }

        #endregion auto
    }
}
