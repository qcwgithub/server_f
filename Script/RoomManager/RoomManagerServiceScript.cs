using Data;

namespace Script
{
    public class RoomManagerServiceScript : ServiceScript<RoomManagerService>
    {
        public RoomManagerServiceScript(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public async Task<ECode> InsertRoomInfo(RoomInfo roomInfo)
        {
            var msgDb = new MsgInsert_RoomInfo();
            msgDb.roomInfo = roomInfo;

            var r = await this.service.dbServiceProxy.Insert_RoomInfo(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InsertRoomInfo({roomInfo.roomId}) r.e {r.e}");
                return r.e;
            }

            return ECode.Success;
        }

        public RoomInfo NewRoomInfo(long roomId, RoomType roomType)
        {
            RoomInfo roomInfo = RoomInfo.Ensure(null);
            roomInfo.roomId = roomId;
            roomInfo.roomType = roomType;

            long nowS = TimeUtils.GetTimeS();
            roomInfo.createTimeS = nowS;
            return roomInfo;
        }
    }
}