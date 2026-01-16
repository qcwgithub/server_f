namespace Data
{
    public sealed class GatewayServiceData : ServiceData
    {
        public readonly Dictionary<long, GatewayUser> userDict = new();
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

        public readonly ObjectLocatorData userLocatorData = new();
        public readonly ObjectLocationAssignmentData userServiceAllocatorData = new();

        public static readonly List<ServiceType> s_connectToServiceIds = new List<ServiceType>
        {
            ServiceType.Global,
            ServiceType.User,
            ServiceType.UserManager,
            ServiceType.Room,
        };

        public GatewayServiceData(ServerData serverData, ServiceTypeAndId serviceTypeAndId)
            : base(serverData, serviceTypeAndId, s_connectToServiceIds)
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