using Data;

namespace Script
{
    public class UserIdSnowflakeScript : SnowflakeScript<UserManagerService>
    {
        public UserIdSnowflakeScript(Server server, UserManagerService service) : base(server, service, service.sd.userIdSnowflakeData)
        {
        }

        public async Task<ECode> InitUserIdSnowflakeData(long workerId)
        {
            var msgDb = new MsgQuery_UserInfo_maxOf_userId();

            var r = await this.service.dbServiceProxy.Query_UserInfo_maxOf_userId(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"Query_UserInfo_maxOf_userId ECode.{r.e}");
                return r.e;
            }

            var resDb = r.CastRes<ResQuery_UserInfo_maxOf_userId>();

            long maxUserId = resDb.result;

            long stamp = this.NowSnowflakeStamp();
            long preStamp;
            long preWorkerId;
            long preSeq;

            if (maxUserId == 0)
            {
                preStamp = stamp;
            }
            else if (!Decode(maxUserId, out preStamp, out preWorkerId, out preSeq))
            {
                this.service.logger.Error($"!Decode(maxUserId {maxUserId}, ...)");
                return ECode.Error;
            }

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

        public long NextUserId()
        {
            return this.NextId();
        }
    }
}