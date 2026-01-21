using Data;

namespace Script
{
    public partial class Service
    {
        public virtual async Task<ECode> OnConnectComplete(IServiceConnection serviceConnection)
        {
            this.logger.Info($"OnConnectComplete to: {serviceConnection.identifierString}");

            // 连上去之后立即向他报告是我的身份
            var msgInfo = new MsgConnectorInfo();
            msgInfo.connectorInfo = this.CreateConnectorInfo();
            await this.GetServiceProxy(serviceConnection.serviceType).ConnectorInfo(serviceConnection.serviceId, msgInfo);

            return ECode.Success;
        }
    }
}