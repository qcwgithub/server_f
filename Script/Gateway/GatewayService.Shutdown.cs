using Data;

namespace Script
{
    public partial class GatewayService
    {
        protected override async Task StopBusinesses()
        {
            this.logger.Info("StopBusinesses");

            //// stop listening for client
            this.sd.StopListenForClient_Tcp();
        }
    }
}