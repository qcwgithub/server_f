using MessagePack;

namespace Data
{
    [MessagePackObject]
    public sealed class UserServiceInfo
    {
        [Key(0)]
        public int serviceId;
        [Key(1)]
        public int userCount;
        [Key(2)]
        public bool allowNewUser;
    }
}