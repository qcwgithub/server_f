using Data;

namespace Script
{
    public partial class RoomService
    {
        protected override async Task<ECode> Start2()
        {
            long workerId = this.sd.serviceConfig.roomMessageIdSnowflakeWorkerId;

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

            ECode e = await this.roomMessageIdSnowflakeScript.InitRoomMessageIdSnowflakeData(workerId);
            if (e != ECode.Success)
            {
                return e;
            }

            await this.UpdateRuntimeInfo();
            return ECode.Success;
        }
    }
}