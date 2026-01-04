using Data;

namespace Script
{
    public partial class RoomManagerService
    {
        protected override async Task<ECode> Start2()
        {
            var serviceConfig = sd.serviceConfig;

            if (serviceConfig.roomIdSnowflakeWorkerId < SnowflakeScript<Service>.MIN_WORKER_ID)
            {
                this.logger.Error($"serviceConfig.roomIdSnowflakeWorkerId < {SnowflakeScript<Service>.MIN_WORKER_ID}");
                return ECode.ServiceConfigError;
            }

            if (serviceConfig.roomIdSnowflakeWorkerId > SnowflakeScript<Service>.MAX_WORKER_ID)
            {
                this.logger.Error($"serviceConfig.roomIdSnowflakeWorkerId > {SnowflakeScript<Service>.MAX_WORKER_ID}");
                return ECode.ServiceConfigError;
            }

            ECode e = await this.roomIdSnowflakeScript.InitRoomIdSnowflakeData(serviceConfig.roomIdSnowflakeWorkerId);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}