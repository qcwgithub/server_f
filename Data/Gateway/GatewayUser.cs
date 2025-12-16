namespace Data
{
    public class GatewayUser
    {
        public long userId;

        public GatewayUserConnection? connection;
        public bool IsConnected()
        {
            return this.connection != null && this.connection.IsConnected();
        }

        public int userServiceId;
        public long offlineTimeS;
        public bool destroying;
        public ITimer? destroyTimer;
    }
}