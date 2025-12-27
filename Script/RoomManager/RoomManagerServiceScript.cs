using Data;

namespace Script
{
    public class RoomManagerServiceScript : ServiceScript<RoomManagerService>
    {
        public RoomManagerServiceScript(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public async Task<(ECode, RoomInfo?)> QueryRoomInfo(long roomId)
        {
            var msgDb = new MsgQuery_RoomInfo_by_roomId();
            msgDb.roomId = roomId;

            var r = await this.service.connectToDbService.Request<MsgQuery_RoomInfo_by_roomId, ResQuery_RoomInfo_by_roomId>(MsgType._Query_RoomInfo_by_roomId, msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"QueryRoomInfo({roomId}) r.err {r.e}");
                return (r.e, null);
            }

            RoomInfo? roomInfo = r.res.result;
            if (roomInfo != null)
            {
                if (roomInfo.roomId != roomId)
                {
                    this.service.logger.Error($"QueryRoomRoomInfo({roomId}) different roomInfo.roomId {roomInfo.roomId}");
                    return (ECode.Error, null);
                }

                roomInfo.Ensure();
            }

            return (ECode.Success, roomInfo);
        }

        public async Task<ECode> InsertRoomInfo(RoomInfo roomInfo)
        {
            var msgDb = new MsgInsert_RoomInfo();
            msgDb.roomInfo = roomInfo;

            var r = await this.service.connectToDbService.Request<MsgInsert_RoomInfo, ResInsert_RoomInfo>(MsgType._Insert_RoomInfo, msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InsertRoomInfo({roomInfo.roomId}) r.e {r.e}");
                return r.e;
            }

            return ECode.Success;
        }

        public RoomInfo NewRoomInfo(long roomId)
        {
            RoomInfo roomInfo = RoomInfo.Ensure(null);
            roomInfo.roomId = roomId;

            long nowS = TimeUtils.GetTimeS();
            roomInfo.createTimeS = nowS;
            return roomInfo;
        }
    }
}