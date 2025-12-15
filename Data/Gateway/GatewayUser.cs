namespace Data
{
    public class GatewayUser
    {
        public long userId;

        public GatewayClientConnection? connection;
        public bool IsConnected()
        {
            return this.connection != null && this.connection.IsConnected();
        }

        public int userServiceId;
    }
}