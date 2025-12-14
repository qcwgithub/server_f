using Data;

namespace Script
{
    public class Auth_Start : OnStart<AuthService>
    {
        public Auth_Start(Server server, AuthService service) : base(server, service)
        {
        }

        protected override async Task<ECode> Handle2()
        {
            long workerId = 0; // TODO
            ECode e = await this.service.userIdSnowflakeScript.InitUserIdSnowflakeData(workerId);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}