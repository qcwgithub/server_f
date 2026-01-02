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

        protected async Task<MyResponse> Request(ServiceType serviceType, MsgType msgType, object msg)
        {
            IConnection? connection = this.self.protocolClientScriptForS.RandomOtherServiceConnection(serviceType);
            if (connection == null)
            {
                return new MyResponse(ECode.Server_NotConnected);
            }

            return await connection.Request(msgType, msg);
        }

        protected async Task<MyResponse> Request(int serviceId, MsgType msgType, object msg)
        {
            IConnection? connection = this.self.data.GetOtherServiceConnection(serviceId);
            if (connection == null || !connection.IsConnected())
            {
                return new MyResponse(ECode.Server_NotConnected);
            }

            if (connection == null)
            {
                return new MyResponse(ECode.Server_NotConnected);
            }

            return await connection.Request(msgType, msg);
        }

        #region auto_proxy

        public async Task<MyResponse> Shutdown(int serviceId, MsgShutdown msg)
        {
            return await this.Request(serviceId, MsgType._Service_Shutdown, msg);
        }
        public async Task<MyResponse> ReloadScript(int serviceId, MsgReloadScript msg)
        {
            return await this.Request(serviceId, MsgType._Service_ReloadScript, msg);
        }
        public async Task<MyResponse> ConnectorInfo(int serviceId, MsgConnectorInfo msg)
        {
            return await this.Request(serviceId, MsgType._Service_ConnectorInfo, msg);
        }
        public async Task<MyResponse> GetPendingMessageList(int serviceId, MsgGetPendingMsgList msg)
        {
            return await this.Request(serviceId, MsgType._Service_GetPendingMessageList, msg);
        }
        public async Task<MyResponse> GetScriptVersion(int serviceId, MsgGetScriptVersion msg)
        {
            return await this.Request(serviceId, MsgType._Service_GetScriptVersion, msg);
        }
        public async Task<MyResponse> ReloadConfigs(int serviceId, MsgReloadConfigs msg)
        {
            return await this.Request(serviceId, MsgType._Service_ReloadConfigs, msg);
        }
        public async Task<MyResponse> GC(int serviceId, MsgGC msg)
        {
            return await this.Request(serviceId, MsgType._Service_GC, msg);
        }
        public async Task<MyResponse> RemoteWillShutdown(int serviceId, MsgRemoteWillShutdown msg)
        {
            return await this.Request(serviceId, MsgType._Service_RemoteWillShutdown, msg);
        }
        public async Task<MyResponse> GetServiceState(int serviceId, MsgGetServiceState msg)
        {
            return await this.Request(serviceId, MsgType._Service_GetServiceState, msg);
        }
        public async Task<MyResponse> GetReloadConfigOptions(int serviceId, MsgGetReloadConfigOptions msg)
        {
            return await this.Request(serviceId, MsgType._Service_GetReloadConfigOptions, msg);
        }
        public async Task<MyResponse> GetConnectedInfos(int serviceId, MsgGetConnectedInfos msg)
        {
            return await this.Request(serviceId, MsgType._Service_GetConnectedInfos, msg);
        }
        public async Task<MyResponse> ViewMongoDumpList(int serviceId, MsgViewMongoDumpList msg)
        {
            return await this.Request(serviceId, MsgType._Service_ViewMongoDumpList, msg);
        }

        #endregion auto_proxy
    }
}