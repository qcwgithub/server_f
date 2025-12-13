using Data;

namespace Script
{
    public class Gateway_Start : OnStart<GatewayService>
    {
        public Gateway_Start(Server server, GatewayService service) : base(server, service)
        {
            
        }

        protected override async Task<ECode> Handle2()
        {
            var sd = this.service.sd;
            var serviceConfig = sd.serviceConfig;

            if (string.IsNullOrEmpty(serviceConfig.outIp))
            {
                this.service.logger.Error("string.IsNullOrEmpty(serviceConfig.outIp)");
                return ECode.ServiceConfigError;
            }

            this.service.data.ListenForClient_Tcp(serviceConfig.outPort);
            return ECode.Success;
        }
    }
}