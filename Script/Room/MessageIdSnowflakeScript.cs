using Data;

namespace Script
{
    public class MessageIdSnowflakeScript : SnowflakeScript<RoomService>
    {
        public MessageIdSnowflakeScript(Server server, RoomService service) : base(server, service, service.sd.messageIdSnowflakeData)
        {
        }

        public async Task<ECode> InitMessageIdSnowflakeData(long workerId)
        {
            long stamp = this.NowSnowflakeStamp();
            long preStamp = stamp;

            if (stamp < preStamp)
            {
                this.service.logger.Error($"stamp {stamp} < preStamp {preStamp}");
                return ECode.Error;
            }

            if (!base.InitSnowflakeData(stamp, workerId))
            {
                this.service.logger.Error($"!base.InitSnowflakeData(stamp {stamp}, workerId {workerId})");
                return ECode.Error;
            }

            return ECode.Success;
        }

        public long NextMessageId()
        {
            return this.NextId();
        }
    }
}