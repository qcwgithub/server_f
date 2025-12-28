namespace Data
{
    public sealed class GatewayServiceData : ServiceData
    {
        public readonly Dictionary<long, GatewayUser> userDict;
        public int userCount
        {
            get
            {
                return this.userDict.Count;
            }
        }
        public GatewayUser? GetUser(long userId)
        {
            return this.userDict.TryGetValue(userId, out GatewayUser? user) ? user : null;
        }
        public bool RemoveUser(long userId)
        {
            return this.userDict.Remove(userId);
        }
        public void AddUser(GatewayUser user)
        {
            this.userDict.Add(user.userId, user);
        }
    
        public int destroyTimeoutS = 600;
        public readonly ObjectLocatorData userLocatorData;
        public readonly ObjectLocationAssignmentData userServiceAllocatorData;

        public static readonly List<ServiceType> s_connectToServiceIds = new List<ServiceType>
        {
            ServiceType.Db,
            ServiceType.Global,
            ServiceType.User,
            ServiceType.UserManager,
        };

        public GatewayServiceData(ServiceTypeAndId serviceTypeAndId)
            : base(serviceTypeAndId, s_connectToServiceIds)
        {
            this.userDict = new Dictionary<long, GatewayUser>();
            this.userLocatorData = new ObjectLocatorData();
            this.userServiceAllocatorData = new ObjectLocationAssignmentData();

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