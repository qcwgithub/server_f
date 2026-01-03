namespace Data
{
    public class RobotUser
    {
        public UserInfo userInfo;
        
        public long userId => this.userInfo.userId;
    }
}