namespace Data
{
    public class GatewayUser
    {
        public readonly long userId;
        public readonly int userServiceId;
        public GatewayUser(long userId, int userServiceId)
        {
            this.userId = userId;
            this.userServiceId = userServiceId;
        }

        public GatewayUserConnection? connection;
    }
}