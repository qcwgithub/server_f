
namespace Data
{
    public class RobotServiceData : ServiceData
    {
        public readonly Dictionary<long, RobotUser> userDict;
        public int userCount
        {
            get
            {
                return this.userDict.Count;
            }
        }
        public RobotUser? GetUser(long userId)
        {
            return this.userDict.TryGetValue(userId, out RobotUser? user) ? user : null;
        }
        public bool RemoveUser(long userId)
        {
            if (this.userDict.Remove(userId))
            {
                return true;
            }
            return false;
        }
        public void AddUser(RobotUser user)
        {
            this.userDict.Add(user.userId, user);
        }

        public static readonly List<ServiceType> s_connectToServiceIds = new List<ServiceType>
        {

        };

        public RobotServiceData(ServiceTypeAndId serviceTypeAndId, List<ServiceType> connectToServiceIds) : base(serviceTypeAndId, connectToServiceIds)
        {
        }

        public readonly ProtocolClientCallbackProviderRobotClient protocolClientCallbackProviderRobotClient = new();
    }
}