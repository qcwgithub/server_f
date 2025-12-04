using Data;

namespace Script
{
    public abstract class UserHandler : Handler<UserService>
    {
        public UserServiceData usData { get { return this.service.sd; } }

        public UserServiceScript usScript { get { return this.service.ss; } }

        public User? GetUser(ProtocolClientData socket)
        {
            object obj = this.service.tcpClientScript.GetUser(socket);
            return (obj == null ? null : (User)obj);
        }
    }
}