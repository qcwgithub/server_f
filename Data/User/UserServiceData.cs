namespace Data
{
    public sealed class UserServiceData : ServiceData
    {
        public readonly Dictionary<long, User> userDict = new();
        public int userCount
        {
            get
            {
                return this.userDict.Count;
            }
        }
        public User? GetUser(long userId)
        {
            return this.userDict.TryGetValue(userId, out User? user) ? user : null;
        }
        public bool RemoveUser(long userId)
        {
            if (this.userDict.Remove(userId))
            {
                this.userCountDelta--;
                return true;
            }
            return false;
        }
        public void AddUser(User user)
        {
            this.userCountDelta++;
            this.userDict.Add(user.userId, user);
        }

        public int userCountDelta = 0;
        public int destroyTimeoutS = 600;
        public int saveIntervalS = 60;
        public bool allowNewUser;

        //------------------------------------------------------

        public static readonly List<ServiceType> s_connectToServiceIds = new List<ServiceType>
        {
            ServiceType.Db,
            ServiceType.Global,
            ServiceType.UserManager,
        };

        public readonly ObjectLocatorData roomLocatorData;

        public UserServiceData(ServiceTypeAndId serviceTypeAndId)
            : base(serviceTypeAndId, s_connectToServiceIds)
        {
            this.LoadConfigs();

            this.allowNewUser = true;
            this.roomLocatorData = new ObjectLocatorData();
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

        public ITimer timer_tick_loop;

        public class LockedUser
        {
            public object? owner;
            public List<TaskCompletionSource>? waiting;
        }
        public readonly Dictionary<long, LockedUser> lockedUserDict = new();
        public bool IsUserLocked(long userId)
        {
            return this.lockedUserDict.TryGetValue(userId, out var lockedUser) && lockedUser.owner != null;
        }
    }
}