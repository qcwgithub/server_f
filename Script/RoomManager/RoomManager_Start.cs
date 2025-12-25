using Data;

namespace Script
{
    public class RoomManager_Start : OnStart<RoomManagerService>
    {
        public RoomManager_Start(Server server, RoomManagerService service) : base(server, service)
        {
        }

        protected override async Task<ECode> Handle2()
        {
            var sd = this.service.sd;
            var serviceConfig = sd.serviceConfig;

            if (serviceConfig.roomIdSnowflakeWorkerId < SnowflakeScript<Service>.MIN_WORKER_ID)
            {
                this.service.logger.Error("serviceConfig.roomIdSnowflakeWorkerId < SnowflakeScript.MIN_WORKER_ID");
                return ECode.ServiceConfigError;
            }

            ECode e = await this.service.roomIdSnowflakeScript.InitRoomIdSnowflakeData(serviceConfig.roomIdSnowflakeWorkerId);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}