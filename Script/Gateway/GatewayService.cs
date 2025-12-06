using Data;

namespace Script
{
    public class GatewayService : Service
    {
        public GatewayServiceData gatewayServiceData
        {
            get
            {
                return (GatewayServiceData)this.data;
            }
        }

        public GatewayService(Server server, int serviceId) : base(server, serviceId)
        {
            
        }
        
        public override void Attach()
        {
            base.Attach();
            base.AddHandler<GatewayService>();
        }
    }
}