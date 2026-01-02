using Data;

namespace Script
{
    public partial class GatewayService
    {
        protected override async Task<ECode> Start2()
        {
            var serviceConfig = this.sd.serviceConfig;

            if (string.IsNullOrEmpty(serviceConfig.outIp))
            {
                this.logger.Error("string.IsNullOrEmpty(serviceConfig.outIp)");
                return ECode.ServiceConfigError;
            }

            this.data.ListenForClient_Tcp(serviceConfig.outPort);
            return ECode.Success;
        }
    }
}