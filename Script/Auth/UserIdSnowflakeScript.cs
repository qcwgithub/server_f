namespace Script
{
    public class UserIdSnowflakeScript : SnowflakeScript<AuthService>
    {
        public UserIdSnowflakeScript(Server server, AuthService service) : base(server, service, service.sd.userIdSnowflakeData)
        {
        }

        public long NextUserId()
        {
            return this.NextId();
        }
    }
}