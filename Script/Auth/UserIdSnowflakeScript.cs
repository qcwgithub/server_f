using Data;

namespace Script
{
    public class UserIdSnowflakeScript : SnowflakeScript<AuthService>
    {
        public UserIdSnowflakeScript(Server server, AuthService service) : base(server, service, service.sd.userIdSnowflakeData)
        {
        }

        public async Task<ECode> InitUserIdSnowflakeData(long workerId)
        {
            var msgDb = new MsgQuery_UserInfo_maxOf_userId();

            var r = await this.service.connectToDbService.Request<MsgQuery_UserInfo_maxOf_userId, ResQuery_UserInfo_maxOf_userId>(MsgType._Query_UserInfo_maxOf_userId, msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InitSnowflakeData r.e {r.e}");
                return r.e;
            }

            long maxUserId = r.res.result;
            if (!Decode(maxUserId, out long preStamp, out long preWorkerId, out long preSeq))
            {
                return ECode.Error;
            }

            long stamp = this.NowSnowflakeStamp();
            if (stamp < preStamp)
            {
                return ECode.Error;
            }

            if (!base.InitSnowflakeData(stamp, workerId))
            {
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