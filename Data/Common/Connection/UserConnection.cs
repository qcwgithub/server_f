namespace Data
{
    public class UserConnection : IClientConnection
    {
        public readonly int gatewayServiceId;
        public readonly User user;
        public readonly UserServiceData sd;
        public UserConnection(int gatewayServiceId, User user, UserServiceData sd)
        {
            this.gatewayServiceId = gatewayServiceId;
            this.user = user;
            this.sd = sd;
        }

        public User? GetUser()
        {
            return this.user;
        }

        public bool IsConnected()
        {
            IServiceConnection? serviceConnection = this.sd.GetOtherServiceConnection(this.gatewayServiceId);
            return serviceConnection != null && serviceConnection.IsConnected();
        }

        public void Send(MsgType msgType, object msg, ReplyCallback? cb)
        {
            IServiceConnection? serviceConnection = this.sd.GetOtherServiceConnection(this.gatewayServiceId);
            if (serviceConnection != null && serviceConnection.IsConnected())
            {
                this.sd.Get_S_to_G().S_to_G(serviceConnection, this.user.userId, msgType, msg, cb);
            }
        }
    }
}