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
                this.service.logger.Error($"InsertSceneInfo({sceneInfo.sceneId}) r.e {r.e}");
                return r.e;
            }

            return ECode.Success;
        }

        public SceneInfo NewSceneInfo(long roomId)
        {
            var sceneInfo = SceneInfo.Ensure(null);
            sceneInfo.sceneId = roomId;

            long nowS = TimeUtils.GetTimeS();
            sceneInfo.createTimeS = nowS;
            return sceneInfo;
        }

        public ECode CheckCreateRoom(MsgRoomManagerCreateRoom msg)
        {
            switch (msg.roomType)
            {
                case RoomType.Private:
                    {
                        if (msg.participants == null || msg.participants.Count < 2)
                        {
                            return ECode.InvalidParam;
                        }

                        for (int i = 0; i < msg.participants.Count; i++)
                        {
                            long userId = msg.participants[i];
                            for (int j = i + 1; j < msg.participants.Count; j++)
                            {
                                if (userId == msg.participants[j])
                                {
                                    return ECode.Duplicate;
                                }
                            }
                        }
                    }
                    break;
                case RoomType.Public:
                    {
                        if (string.IsNullOrEmpty(msg.title))
                        {
                            return ECode.InvalidParam;
                        }
                        if (string.IsNullOrEmpty(msg.desc))
                        {
                            return ECode.InvalidParam;
                        }
                    }
                    break;
                default:
                    throw new Exception($"Not handled roomType.{msg.roomType}");
            }

            return ECode.Success;
        }
    }
}