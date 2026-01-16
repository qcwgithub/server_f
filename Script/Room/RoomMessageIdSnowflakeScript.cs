using Data;

namespace Script
{
    public class RoomMessageIdSnowflakeScript : SnowflakeScript<RoomService>
    {
        public RoomMessageIdSnowflakeScript(Server server, RoomService service) : base(server, service, service.sd.roomMessageIdSnowflakeData)
        {
        }

        public async Task<ECode> InitRoomMessageIdSnowflakeData(long workerId)
        {
            long stamp = this.NowSnowflakeStamp();

            if (!base.InitSnowflakeData(stamp, workerId))
            {
                this.service.logger.Error($"!base.InitSnowflakeData(stamp {stamp}, workerId {workerId})");
                return ECode.Error;
            }

            return ECode.Success;
        }

        public long NextRoomMessageId()
        {
            return this.NextId();
        }
    }
}