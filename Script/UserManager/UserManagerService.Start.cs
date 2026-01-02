using Data;

namespace Script
{
    public partial class UserManagerService
    {
        protected override async Task<ECode> Start2()
        {
            var serviceConfig = this.sd.serviceConfig;

            if (serviceConfig.userIdSnowflakeWorkerId < SnowflakeScript<Service>.MIN_WORKER_ID)
            {
                this.logger.Error("serviceConfig.userIdSnowflakeWorkerId < SnowflakeScript.MIN_WORKER_ID");
                return ECode.ServiceConfigError;
            }

            ECode e = await this.userIdSnowflakeScript.InitUserIdSnowflakeData(serviceConfig.userIdSnowflakeWorkerId);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}