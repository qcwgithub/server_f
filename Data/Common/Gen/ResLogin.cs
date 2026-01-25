using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResLogin
    {
        [Key(0)]
        public bool isNewUser;
        [Key(1)]
        public UserInfo userInfo;
        [Key(2)]
        public bool kickOther;
    }
}
