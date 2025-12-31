using Data;

namespace Script
{
    public class ServiceProxy
    {
        public readonly Service self;
        public readonly ServiceType to;
        public ServiceProxy(Service self, ServiceType to)
        {
            this.self = self;
            this.to = to;
        }

        protected async Task<MyResponse<Res>> Request<Msg, Res>(ServiceType serviceType, MsgType msgType, Msg msg)
            where Res : class
        {
            IConnection? connection = this.self.protocolClientScriptForS.RandomOtherServiceConnection(serviceType);
            if (connection == null)
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }

            return await connection.Request<Msg, Res>(msgType, msg);
        }

        protected async Task<MyResponse<Res>> Request<Msg, Res>(int serviceId, MsgType msgType, Msg msg)
            where Res : class
        {
            IConnection? connection = this.self.data.GetOtherServiceConnection(serviceId);
            if (connection == null || !connection.IsConnected())
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }

            if (connection == null)
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }

            return await connection.Request<Msg, Res>(msgType, msg);
        }

        #region auto_proxy

        public async Task<MyResponse<ResReloadScript>> ReloadScript(int serviceId, MsgReloadScript msg)
        {
            return await this.Request<MsgReloadScript, ResReloadScript>(serviceId, MsgType._Service_ReloadScript, msg);
        }
        public async Task<MyResponse<ResConnectorInfo>> ConnectorInfo(int serviceId, MsgConnectorInfo msg)
        {
            return await this.Request<MsgConnectorInfo, ResConnectorInfo>(serviceId, MsgType._Service_ConnectorInfo, msg);
        }
        public async Task<MyResponse<ResGetPendingMsgList>> GetPendingMessageList(int serviceId, MsgGetPendingMsgList msg)
        {
            return await this.Request<MsgGetPendingMsgList, ResGetPendingMsgList>(serviceId, MsgType._Service_GetPendingMessageList, msg);
        }
        public async Task<MyResponse<ResGetScriptVersion>> GetScriptVersion(int serviceId, MsgGetScriptVersion msg)
        {
            return await this.Request<MsgGetScriptVersion, ResGetScriptVersion>(serviceId, MsgType._Service_GetScriptVersion, msg);
        }
        public async Task<MyResponse<ResReloadConfigs>> ReloadConfigs(int serviceId, MsgReloadConfigs msg)
        {
            return await this.Request<MsgReloadConfigs, ResReloadConfigs>(serviceId, MsgType._Service_ReloadConfigs, msg);
        }
        public async Task<MyResponse<ResGC>> GC(int serviceId, MsgGC msg)
        {
            return await this.Request<MsgGC, ResGC>(serviceId, MsgType._Service_GC, msg);
        }
        public async Task<MyResponse<ResRemoteWillShutdown>> RemoteWillShutdown(int serviceId, MsgRemoteWillShutdown msg)
        {
            return await this.Request<MsgRemoteWillShutdown, ResRemoteWillShutdown>(serviceId, MsgType._Service_RemoteWillShutdown, msg);
        }
        public async Task<MyResponse<ResGetServiceState>> GetServiceState(int serviceId, MsgGetServiceState msg)
        {
            return await this.Request<MsgGetServiceState, ResGetServiceState>(serviceId, MsgType._Service_GetServiceState, msg);
        }
        public async Task<MyResponse<ResGetReloadConfigOptions>> GetReloadConfigOptions(int serviceId, MsgGetReloadConfigOptions msg)
        {
            return await this.Request<MsgGetReloadConfigOptions, ResGetReloadConfigOptions>(serviceId, MsgType._Service_GetReloadConfigOptions, msg);
        }
        public async Task<MyResponse<ResGetConnectedInfos>> GetConnectedInfos(int serviceId, MsgGetConnectedInfos msg)
        {
            return await this.Request<MsgGetConnectedInfos, ResGetConnectedInfos>(serviceId, MsgType._Service_GetConnectedInfos, msg);
        }
        public async Task<MyResponse<ResViewMongoDumpList>> ViewMongoDumpList(int serviceId, MsgViewMongoDumpList msg)
        {
            return await this.Request<MsgViewMongoDumpList, ResViewMongoDumpList>(serviceId, MsgType._Service_ViewMongoDumpList, msg);
        }

        #endregion auto_proxy
    }
}