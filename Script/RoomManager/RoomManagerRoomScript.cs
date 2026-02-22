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

        public async Task<ECode> InsertPrivateSceneInfo(FriendChatInfo privateRoomInfo)
        {
            var msgDb = new MsgInsert_FriendChatInfo();
            msgDb.privateRoomInfo = privateRoomInfo;

            var r = await this.service.dbServiceProxy.Insert_FriendChatInfo(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InsertPrivateSceneInfo({privateRoomInfo.roomId}) r.e {r.e}");
                return r.e;
            }

            return ECode.Success;
        }

        public FriendChatInfo NewPrivateRoomInfo(long roomId)
        {
            var privateRoomInfo = FriendChatInfo.Ensure(null);
            privateRoomInfo.roomId = roomId;

            long nowS = TimeUtils.GetTimeS();
            privateRoomInfo.createTimeS = nowS;
            return privateRoomInfo;
        }
    }
}