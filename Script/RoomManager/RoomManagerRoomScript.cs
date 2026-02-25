using Data;

namespace Script
{
    public class RoomManagerRoomScript : ServiceScript<RoomManagerService>
    {
        public RoomManagerRoomScript(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public async Task<ECode> InsertSceneInfo(SceneInfo sceneInfo)
        {
            var msgDb = new MsgInsert_SceneInfo();
            msgDb.sceneInfo = sceneInfo;

            var r = await this.service.dbServiceProxy.Insert_SceneInfo(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InsertSceneInfo({sceneInfo.roomId}) r.e {r.e}");
                return r.e;
            }

            return ECode.Success;
        }

        public SceneInfo NewSceneInfo(long roomId)
        {
            var sceneInfo = SceneInfo.Ensure(null);
            sceneInfo.roomId = roomId;

            long nowS = TimeUtils.GetTimeS();
            sceneInfo.createTimeS = nowS;
            return sceneInfo;
        }

        public async Task<ECode> InsertFriendChatRoomInfo(FriendChatRoomInfo roomInfo)
        {
            var msgDb = new MsgInsert_FriendChatRoomInfo();
            msgDb.roomInfo = roomInfo;

            var r = await this.service.dbServiceProxy.Insert_FriendChatRoomInfo(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InsertPrivateSceneInfo({roomInfo.roomId}) r.e {r.e}");
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