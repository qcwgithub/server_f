using System;
using System.Collections.Generic;
using log4net;

namespace Data
{
    public sealed class GatewayServiceData : ServiceData
    {
        public static readonly List<ServiceType> s_connectToServiceIds = new List<ServiceType>
        {
            ServiceType.Database,
            ServiceType.Global,
        };

        public GatewayServiceData(ServiceTypeAndId serviceTypeAndId)
            : base(serviceTypeAndId, s_connectToServiceIds)
        {
            this.LoadConfigs();
        }

        void LoadConfigs()
        {
            
        }

        public override void ReloadConfigs(bool all, List<string> files)
        {
            // this.configLoader.Reset_playerSpecialProfiles();
            if (all)
            {
                this.LoadConfigs();
            }
            else
            {
                
            }
        }

        public override void GetReloadConfigOptions(List<string> files)
        {
            files.Add("all");
        }
    }
}