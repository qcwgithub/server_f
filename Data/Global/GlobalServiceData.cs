using System;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class GlobalServiceData : ServiceData
    {
        public static readonly List<ServiceType> s_connectToServiceIds = new List<ServiceType>
        {

        };
        public ITimer timer_tick_Loop;

        public GlobalServiceData(ServiceTypeAndId serviceTypeAndId)
            : base(serviceTypeAndId, s_connectToServiceIds)
        {
            this.LoadConfigs(false);
        }

        public List<ServiceConfig> allServiceConfigs;
        public ProfileGlobal profileGlobal;

        public void LoadConfigs(bool isReload)
        {
            this.LoadAllServiceConfigs(isReload);
        }

        void LoadAllServiceConfigs(bool isReload)
        {
            if (ServerData.instance.configLoader.LoadAllServiceConfigs(out List<ServiceConfig> alln, out string message))
            {
                this.allServiceConfigs = alln;
            }
            else
            {
                if (!isReload)
                {
                    throw new Exception(message);
                }
                else
                {
                    this.logger.Error(message);
                }
            }
        }

        public override void ReloadConfigs(bool all, List<string> files)
        {
            if (all)
            {
                this.LoadConfigs(true);
            }
            else
            {
                if (files.Contains(ConfigLoader.AllServiceConfigsFile))
                {
                    this.LoadAllServiceConfigs(true);
                }
            }
        }

        public override void GetReloadConfigOptions(List<string> files)
        {
            files.Add("all");
            files.Add(ConfigLoader.AllServiceConfigsFile);
        }
    }
}