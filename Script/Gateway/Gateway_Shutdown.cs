using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Gateway_Shutdown : OnShutdown<GatewayService>
    {
        public Gateway_Shutdown(Server server, GatewayService service) : base(server, service)
        {
        }

        protected override async Task StopBusinesses()
        {
            this.service.logger.Info("StopBusinesses");

            GatewayServiceData sd = this.service.sd;

            //// stop listening for client
            sd.StopListenForClient_Tcp();
        }
    }
}