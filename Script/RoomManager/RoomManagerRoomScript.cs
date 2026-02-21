using Data;

namespace Script
{
    public class RoomManagerRoomScript : ServiceScript<RoomManagerService>
    {
        public RoomManagerRoomScript(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public async Task<ECode> InsertSceneRoomInfo(SceneRoomInfo sceneRoomInfo)
        {
            var msgDb = new MsgInsert_SceneRoomInfo();
            msgDb.sceneRoomInfo = sceneRoomInfo;

            var r = await this.service.dbServiceProxy.Insert_SceneRoomInfo(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InsertSceneRoomInfo({sceneRoomInfo.roomId}) r.e {r.e}");
                return r.e;
            }

            return ECode.Success;
        }

        public SceneRoomInfo NewSceneRoomInfo(long roomId)
        {
            var sceneRoomInfo = SceneRoomInfo.Ensure(null);
            sceneRoomInfo.roomId = roomId;

            long nowS = TimeUtils.GetTimeS();
            sceneRoomInfo.createTimeS = nowS;
            return sceneRoomInfo;
        }

        public async Task<ECode> InsertPrivateSceneRoomInfo(PrivateRoomInfo privateRoomInfo)
        {
            var msgDb = new MsgInsert_PrivateRoomInfo();
            msgDb.privateRoomInfo = privateRoomInfo;

            var r = await this.service.dbServiceProxy.Insert_PrivateRoomInfo(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InsertPrivateSceneRoomInfo({privateRoomInfo.roomId}) r.e {r.e}");
                return r.e;
            }

            return ECode.Success;
        }

        public PrivateRoomInfo NewPrivateRoomInfo(long roomId)
        {
            var privateRoomInfo = PrivateRoomInfo.Ensure(null);
            privateRoomInfo.roomId = roomId;

            long nowS = TimeUtils.GetTimeS();
            privateRoomInfo.createTimeS = nowS;
            return privateRoomInfo;
        }
    }
}