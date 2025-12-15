using Data;

namespace Script
{
    public class UserManager_Start : OnStart<UserManagerService>
    {
        public UserManager_Start(Server server, UserManagerService service) : base(server, service)
        {
        }

        protected override async Task<ECode> Handle2()
        {
            var sd = this.service.sd;
            var serviceConfig = sd.serviceConfig;

            if (serviceConfig.userIdSnowflakeWorkerId < SnowflakeScript<Service>.MIN_WORKER_ID)
            {
                this.service.logger.Error("serviceConfig.userIdSnowflakeWorkerId < SnowflakeScript.MIN_WORKER_ID");
                return ECode.ServiceConfigError;
            }

            ECode e = await this.service.userIdSnowflakeScript.InitUserIdSnowflakeData(serviceConfig.userIdSnowflakeWorkerId);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}