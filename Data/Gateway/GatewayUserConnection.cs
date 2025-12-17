namespace Data
{
    public class GatewayUserConnection : DirectConnection
    {
        public GatewayUser? user;
        public long userId;
        public long lastUserId;

        public GatewayUserConnection(ProtocolClientData socket) : base(socket, false)
        {

        }

        public void BindUser(GatewayUser user)
        {
            this.user = user;
            this.userId = user.userId;
            this.lastUserId = user.userId;
        }

        public void UnbindUser()
        {
            this.user = null;
            this.userId = 0;
        }

        public GatewayUser? GetUser()
        {
            return this.user;
        }
    }
}