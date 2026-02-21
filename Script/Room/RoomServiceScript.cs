using System.Numerics;
using Data;
using Script;

namespace Script
{
    public class RoomServiceScript : ServiceScript<RoomService>
    {
        public RoomServiceScript(Server server, RoomService service) : base(server, service)
        {
        }

        public async Task<(ECode, SceneInfo?)> QuerySceneInfo(long roomId)
        {
            var msgDb = new MsgQuery_SceneInfo_by_roomId();
            msgDb.roomId = roomId;

            var r = await this.service.dbServiceProxy.Query_SceneInfo_by_roomId(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"QuerySceneInfo({roomId}) r.err {r.e}");
                return (r.e, null);
            }

            var resDb = r.CastRes<ResQuery_SceneInfo_by_roomId>();

            SceneInfo? sceneInfo = resDb.result;
            if (sceneInfo != null)
            {
                if (sceneInfo.roomId != roomId)
                {
                    this.service.logger.Error($"QuerySceneInfo({roomId}) different sceneInfo.roomId {sceneInfo.roomId}");
                    return (ECode.Error, null);
                }

                sceneInfo.Ensure();
            }

            return (ECode.Success, sceneInfo);
        }

        public async Task<(ECode, Room?)> LoadSceneRoom(long roomId)
        {
            (ECode e, SceneInfo? sceneInfo) = await this.QuerySceneInfo(roomId);
            if (e != ECode.Success)
            {
                return (e, null);
            }

            if (sceneInfo == null)
            {
                return (ECode.RoomNotExist, null);
            }

            var room = new Room(sceneInfo);

            await this.server.roomLocationRedisW.WriteLocation(roomId, this.service.serviceId, this.service.sd.saveIntervalS + 60);

            this.AddRoomToDict(room);

            if (!room.saveTimer.IsAlive())
            {
                this.service.ss.SetSaveTimer(room);
            }

            this.service.ss.ClearDestroyTimer(room, RoomClearDestroyTimerReason.RoomLoginSuccess);
            var roomMessageConfig = this.server.data.serverConfig.sceneMessageConfig;

            //
            List<ChatMessage> recents = await this.server.roomMessagesRedis.GetRecents(roomId, roomMessageConfig.recentMessagesCount);
            this.service.logger.Info($"LoadRoom recent messages count {recents.Count}");
            foreach (ChatMessage message in recents)
            {
                room.recentMessages.Enqueue(message);
            }

            return (ECode.Success, room);
        }

        public async Task<(ECode, PrivateRoomInfo?)> QueryPrivateRoomInfo(long roomId)
        {
            var msgDb = new MsgQuery_PrivateRoomInfo_by_roomId();
            msgDb.roomId = roomId;

            var r = await this.service.dbServiceProxy.Query_PrivateRoomInfo_by_roomId(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"QueryPrivateRoomInfo({roomId}) r.err {r.e}");
                return (r.e, null);
            }

            var resDb = r.CastRes<ResQuery_PrivateRoomInfo_by_roomId>();

            PrivateRoomInfo? privateRoomInfo = resDb.result;
            if (privateRoomInfo != null)
            {
                if (privateRoomInfo.roomId != roomId)
                {
                    this.service.logger.Error($"QueryPrivateRoomInfo({roomId}) different privateRoomInfo.roomId {privateRoomInfo.roomId}");
                    return (ECode.Error, null);
                }

                privateRoomInfo.Ensure();
            }

            return (ECode.Success, privateRoomInfo);
        }

        public async Task<(ECode, Room?)> LoadPrivateRoom(long roomId)
        {
            (ECode e, PrivateRoomInfo? privateRoomInfo) = await this.QueryPrivateRoomInfo(roomId);
            if (e != ECode.Success)
            {
                return (e, null);
            }

            if (privateRoomInfo == null)
            {
                return (ECode.RoomNotExist, null);
            }

            var room = new Room(privateRoomInfo);

            await this.server.roomLocationRedisW.WriteLocation(roomId, this.service.serviceId, this.service.sd.saveIntervalS + 60);

            this.AddRoomToDict(room);

            if (!room.saveTimer.IsAlive())
            {
                this.service.ss.SetSaveTimer(room);
            }

            this.service.ss.ClearDestroyTimer(room, RoomClearDestroyTimerReason.RoomLoginSuccess);
            var roomMessageConfig = this.server.data.serverConfig.sceneMessageConfig;

            //
            List<ChatMessage> recents = await this.server.roomMessagesRedis.GetRecents(roomId, roomMessageConfig.recentMessagesCount);
            this.service.logger.Info($"LoadRoom recent messages count {recents.Count}");
            foreach (ChatMessage message in recents)
            {
                room.recentMessages.Enqueue(message);
            }

            return (ECode.Success, room);
        }

        void AddRoomToDict(Room room)
        {
            // runtime 初始化
            this.service.sd.AddRoom(room);

            // 有值就不能再赋值了，不然玩家上线下线就错了
            MyDebug.Assert(room.lastSceneInfo == null);

            room.lastSceneInfo = SceneInfo.Ensure(null);
            room.lastSceneInfo.DeepCopyFrom(room.sceneInfo);

            // qiucw
            // 这句会修改 sceneInfo，必须放在 lastSceneInfo.DeepCopyFrom 后面
            // this.gameScripts.CallInit(room);
            this.service.CheckUpdateRuntimeInfo().Forget();
        }

        public void SetSaveTimer(Room room)
        {
            if (room.saveTimer.IsAlive())
            {
                return;
            }

            var SEC = this.service.sd.saveIntervalS;
#if DEBUG
            SEC = 3;
#endif

            room.saveTimer = server.timerScript.SetLoopTimer(this.service.serviceId, SEC, TimerType.SaveRoom, new TimerSaveRoom
            {
                roomId = room.roomId,
                reason = "SetSaveTimer",
            });
        }

        public void ClearSaveTimer(Room room)
        {
            if (room.saveTimer == null)
            {
                return;
            }

            if (!room.saveTimer.IsAlive())
            {
                return;
            }

            server.timerScript.ClearTimer(room.saveTimer);
            room.saveTimer = null;
        }

        public void SetDestroyTimer(Room room, RoomDestroyRoomReason reason)
        {
            if (room.destroyTimer.IsAlive())
            {
                return;
            }

            var SEC = this.service.sd.destroyTimeoutS;
            this.service.logger.Info($"SetDestroyTimer roomId {room.roomId} reason {reason}");

            room.destroyTimer = this.server.timerScript.SetTimer(
                this.service.serviceId,
                SEC, TimerType.DestroyRoom,
                new TimerDestroyRoom
                {
                    roomId = room.roomId,
                    reason = reason
                });
        }

        public void ClearDestroyTimer(Room room, RoomClearDestroyTimerReason reason)
        {
            if (room.destroyTimer == null)
            {
                return;
            }

            if (room.destroyTimer.IsAlive())
            {
                return;
            }

            this.service.logger.Info($"ClearDestroyTimer roomId {room.roomId} reason {reason}");
            server.timerScript.ClearTimer(room.destroyTimer);
            room.destroyTimer = null;
        }
    }
}