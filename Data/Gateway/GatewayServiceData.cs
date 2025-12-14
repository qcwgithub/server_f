namespace Data
{
    public sealed class GatewayServiceData : ServiceData
    {
        public static readonly List<ServiceType> s_connectToServiceIds = new List<ServiceType>
        {
            ServiceType.Db,
            ServiceType.Global,
            ServiceType.User,
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