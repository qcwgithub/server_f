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
            IConnection? connection = this.self.connectionCallbackScript.RandomOtherServiceConnection(serviceType);
            if (connection == null)
            {
                return new MyResponse(ECode.NotConnected);
            }

            return await connection.Request(msgType, msg);
        }

        public async Task<MyResponse> Request(int serviceId, MsgType msgType, object msg)
        {
            IConnection? connection = this.self.data.GetOtherServiceConnection(serviceId);
            if (connection == null || !connection.IsConnected())
            {
                return new MyResponse(ECode.NotConnected);
            }

            if (connection == null)
            {
                return new MyResponse(ECode.NotConnected);
            }

            return await connection.Request(msgType, msg);
        }

        #region auto_proxy

        public async Task<MyResponse> Shutdown(int serviceId, MsgShutdown msg)
        {
            return await this.Request(serviceId, MsgType._Shutdown, msg);
        }
        public async Task<MyResponse> ReloadScript(int serviceId, MsgReloadScript msg)
        {
            return await this.Request(serviceId, MsgType._ReloadScript, msg);
        }
        public async Task<MyResponse> ConnectorInfo(int serviceId, MsgConnectorInfo msg)
        {
            return await this.Request(serviceId, MsgType._ConnectorInfo, msg);
        }
        public async Task<MyResponse> GetPendingMessageList(int serviceId, MsgGetPendingMsgList msg)
        {
            return await this.Request(serviceId, MsgType._GetPendingMessageList, msg);
        }
        public async Task<MyResponse> GetScriptVersion(int serviceId, MsgGetScriptVersion msg)
        {
            return await this.Request(serviceId, MsgType._GetScriptVersion, msg);
        }
        public async Task<MyResponse> ReloadConfigs(int serviceId, MsgReloadConfigs msg)
        {
            return await this.Request(serviceId, MsgType._ReloadConfigs, msg);
        }
        public async Task<MyResponse> GC(int serviceId, MsgGC msg)
        {
            return await this.Request(serviceId, MsgType._GC, msg);
        }
        public async Task<MyResponse> RemoteWillShutdown(int serviceId, MsgRemoteWillShutdown msg)
        {
            return await this.Request(serviceId, MsgType._RemoteWillShutdown, msg);
        }
        public async Task<MyResponse> GetServiceState(int serviceId, MsgGetServiceState msg)
        {
            return await this.Request(serviceId, MsgType._GetServiceState, msg);
        }
        public async Task<MyResponse> GetReloadConfigOptions(int serviceId, MsgGetReloadConfigOptions msg)
        {
            return await this.Request(serviceId, MsgType._GetReloadConfigOptions, msg);
        }
        public async Task<MyResponse> GetConnectedInfos(int serviceId, MsgGetConnectedInfos msg)
        {
            return await this.Request(serviceId, MsgType._GetConnectedInfos, msg);
        }
        public async Task<MyResponse> ViewMongoDumpList(int serviceId, MsgViewMongoDumpList msg)
        {
            return await this.Request(serviceId, MsgType._ViewMongoDumpList, msg);
        }

        #endregion auto_proxy
    }
}