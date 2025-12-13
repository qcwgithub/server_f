using Data;

namespace Script
{
    public class AuthService : Service
    {
        public AuthServiceData sd
        {
            get
            {
                return (AuthServiceData)this.data;
            }
        }

        public readonly UserIdSnowflakeScript userIdSnowflakeScript;

        public AuthService(Server server, int serviceId) : base(server, serviceId)
        {
            this.userIdSnowflakeScript = new UserIdSnowflakeScript(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<AuthService>();

            this.dispatcher.AddHandler(new Auth_Start(this.server, this));
            this.dispatcher.AddHandler(new Auth_Shutdown(this.server, this));
        }
    }
}