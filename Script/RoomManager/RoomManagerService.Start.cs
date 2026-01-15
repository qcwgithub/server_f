using Data;

namespace Script
{
    public partial class RoomManagerService
    {
        protected override async Task<ECode> Start2()
        {
            long workerId = this.sd.serviceConfig.roomIdSnowflakeWorkerId;

            if (workerId < SnowflakeScript<Service>.MIN_WORKER_ID)
            {
                this.logger.Error($"serviceConfig.roomIdSnowflakeWorkerId < {SnowflakeScript<Service>.MIN_WORKER_ID}");
                return ECode.ServiceConfigError;
            }

            if (workerId > SnowflakeScript<Service>.MAX_WORKER_ID)
            {
                this.logger.Error($"serviceConfig.roomIdSnowflakeWorkerId > {SnowflakeScript<Service>.MAX_WORKER_ID}");
                return ECode.ServiceConfigError;
            }

            ECode e = await this.roomIdSnowflakeScript.InitRoomIdSnowflakeData(workerId);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}