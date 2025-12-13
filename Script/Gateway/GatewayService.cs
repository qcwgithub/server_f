using Data;

namespace Script
{
    public class GatewayService : Service
    {
        public GatewayServiceData sd
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

            this.dispatcher.AddHandler(new Gateway_Start(this.server, this));
            this.dispatcher.AddHandler(new Gateway_Shutdown(this.server, this));
        }
    }
}