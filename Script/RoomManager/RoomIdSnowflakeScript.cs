using Data;

namespace Script
{
    public class RoomIdSnowflakeScript : SnowflakeScript<RoomManagerService>
    {
        public RoomIdSnowflakeScript(Server server, RoomManagerService service) : base(server, service, service.sd.roomIdSnowflakeData)
        {
        }

        public async Task<ECode> InitRoomIdSnowflakeData(long workerId)
        {
            var msgDb = new MsgQuery_SceneInfo_maxOf_roomId();

            var r = await this.service.dbServiceProxy.Query_SceneInfo_maxOf_roomId(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"Query_SceneInfo_maxOf_roomId ECode.{r.e}");
                return r.e;
            }

            var resDb = r.CastRes<ResQuery_SceneInfo_maxOf_roomId>();

            long maxRoomId = resDb.result;

            long stamp = this.NowSnowflakeStamp();
            long preStamp;
            long preWorkerId;
            long preSeq;

            if (maxRoomId == 0)
            {
                preStamp = stamp;
            }
            else if (!Decode(maxRoomId, out preStamp, out preWorkerId, out preSeq))
            {
                this.service.logger.Error($"!Decode(maxRoomId {maxRoomId}, ...)");
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

        public long NextRoomId()
        {
            return this.NextId();
        }
    }
}