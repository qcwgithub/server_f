using Data;

namespace Script
{
    public class RoomManagerRoomScript : ServiceScript<RoomManagerService>
    {
        public RoomManagerRoomScript(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public async Task<ECode> InsertSceneRoomInfo(SceneRoomInfo roomInfo)
        {
            var msgDb = new MsgInsert_SceneRoomInfo();
            msgDb.roomInfo = roomInfo;

            var r = await this.service.dbServiceProxy.Insert_SceneRoomInfo(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InsertSceneRoomInfo({roomInfo.roomId}) r.e {r.e}");
                return r.e;
            }

            return ECode.Success;
        }

        public SceneRoomInfo NewSceneRoomInfo(long roomId)
        {
            var roomInfo = SceneRoomInfo.Ensure(null);
            roomInfo.roomId = roomId;

            long nowS = TimeUtils.GetTimeS();
            roomInfo.createTimeS = nowS;
            return roomInfo;
        }

        public async Task<ECode> InsertFriendChatRoomInfo(FriendChatRoomInfo roomInfo)
        {
            var msgDb = new MsgInsert_FriendChatRoomInfo();
            msgDb.roomInfo = roomInfo;

            var r = await this.service.dbServiceProxy.Insert_FriendChatRoomInfo(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InsertFriendChatRoomInfo({roomInfo.roomId}) r.e {r.e}");
                return r.e;
            }

            return ECode.Success;
        }

        public FriendChatRoomInfo NewFriendChatRoomInfo(long roomId)
        {
            var roomInfo = FriendChatRoomInfo.Ensure(null);
            roomInfo.roomId = roomId;

            long nowS = TimeUtils.GetTimeS();
            roomInfo.createTimeS = nowS;
            return roomInfo;
        }
    }
}