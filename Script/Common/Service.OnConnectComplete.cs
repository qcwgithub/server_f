using Data;

namespace Script
{
    public partial class Service
    {
        public virtual async Task<ECode> OnConnectComplete(IConnection connection)
        {
            if (connection is ServiceConnection serviceConnection)
            {
                this.logger.InfoFormat("OnConnectComplete connection id: {0}, to: {1}", serviceConnection.GetConnectionId(), serviceConnection.tai.ToString());

                // 连上去之后立即向他报告是我的身份
                var msgInfo = new MsgConnectorInfo();
                msgInfo.connectorInfo = this.CreateConnectorInfo();
                await this.GetServiceProxy(serviceConnection.serviceType).ConnectorInfo(serviceConnection.serviceId, msgInfo);
            }

            return ECode.Success;
        }
    }
}