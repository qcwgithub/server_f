using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;


namespace Script
{
    public class User_Start : OnStart<UserService>
    {
        protected override async Task<ECode> Handle2()
        {
            var sd = this.service.usData;
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