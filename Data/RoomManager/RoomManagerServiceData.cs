namespace Data
{
    public class RoomManagerServiceData : ServiceData
    {
        public static readonly List<ServiceType> s_connectToServiceIds = new List<ServiceType>
        {
            ServiceType.Global,
        };

        public readonly SnowflakeData roomIdSnowflakeData;

        public RoomManagerServiceData(ServiceTypeAndId serviceTypeAndId)
            : base(serviceTypeAndId, s_connectToServiceIds)
        {
            this.LoadConfigs(false);
            
            this.roomIdSnowflakeData = new SnowflakeData();
        }

        public void LoadConfigs(bool isReload)
        {

        }

        public override void ReloadConfigs(bool all, List<string> files)
        {
            if (all)
            {
                this.LoadConfigs(true);
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