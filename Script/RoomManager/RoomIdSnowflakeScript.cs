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
            var msgDb = new MsgQuery_RoomInfo_maxOf_roomId();

            var r = await this.service.dbServiceProxy.Query_RoomInfo_maxOf_roomId(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InitSnowflakeData r.e {r.e}");
                return r.e;
            }
            var resDb = r.CastRes<ResQuery_RoomInfo_maxOf_roomId>();

            long maxRoomId = resDb.result;
            if (!Decode(maxRoomId, out long preStamp, out long preWorkerId, out long preSeq))
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

        public long NextRoomId()
        {
            return this.NextId();
        }
    }
}