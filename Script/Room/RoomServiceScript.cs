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

        public async Task<(ECode, SceneRoomInfo?)> QuerySceneRoomInfo(long roomId)
        {
            var msgDb = new MsgQuery_SceneRoomInfo_by_roomId();
            msgDb.roomId = roomId;

            var r = await this.service.dbServiceProxy.Query_SceneRoomInfo_by_roomId(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"QuerySceneRoomInfo({roomId}) r.err {r.e}");
                return (r.e, null);
            }

            var resDb = r.CastRes<ResQuery_SceneRoomInfo_by_roomId>();

            SceneRoomInfo? roomInfo = resDb.result;
            if (roomInfo != null)
            {
                if (roomInfo.roomId != roomId)
                {
                    this.service.logger.Error($"QuerySceneRoomInfo({roomId}) different sceneRoomInfo.roomId {roomInfo.roomId}");
                    return (ECode.Error, null);
                }

                roomInfo.Ensure();
            }

            return (ECode.Success, roomInfo);
        }

        public async Task<(ECode, SceneRoom?)> LoadSceneRoom(long roomId)
        {
            (ECode e, SceneRoomInfo? roomInfo) = await this.QuerySceneRoomInfo(roomId);
            if (e != ECode.Success)
            {
                return (e, null);
            }

            if (roomInfo == null)
            {
                return (ECode.RoomNotExist, null);
            }

            var room = new SceneRoom(roomInfo);

            await this.server.roomLocationRedisW.WriteLocation(roomId, this.service.serviceId, this.service.sd.saveIntervalS + 60);

            this.AddRoomToDict(room);

            if (!room.saveTimer.IsAlive())
            {
                this.service.ss.SetSaveTimer(room);
            }

            this.service.ss.ClearDestroyTimer(room, RoomClearDestroyTimerReason.RoomLoginSuccess);
            var roomMessageConfig = this.server.data.serverConfig.sceneMessageConfig;

            //
            List<ChatMessage> recents = await this.server.sceneMessagesRedis.GetRecents(roomId, roomMessageConfig.recentMessagesCount);
            this.service.logger.Info($"LoadRoom recent messages count {recents.Count}");
            foreach (ChatMessage message in recents)
            {
                room.recentMessages.Enqueue(message);
            }

            return (ECode.Success, room);
        }

        public async Task<(ECode, FriendChatRoomInfo?)> QueryFriendChatRoomInfo(long roomId)
        {
            var msgDb = new MsgQuery_FriendChatRoomInfo_by_roomId();
            msgDb.roomId = roomId;

            var r = await this.service.dbServiceProxy.Query_FriendChatRoomInfo_by_roomId(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"QueryFriendChatRoomInfo({roomId}) r.err {r.e}");
                return (r.e, null);
            }

            var resDb = r.CastRes<ResQuery_FriendChatRoomInfo_by_roomId>();

            FriendChatRoomInfo? friendChatRoomInfo = resDb.result;
            if (friendChatRoomInfo != null)
            {
                if (friendChatRoomInfo.roomId != roomId)
                {
                    this.service.logger.Error($"QueryFriendChatRoomInfo({roomId}) different friendChatRoomInfo.roomId {friendChatRoomInfo.roomId}");
                    return (ECode.Error, null);
                }

                friendChatRoomInfo.Ensure();
            }

            return (ECode.Success, friendChatRoomInfo);
        }

        public async Task<(ECode, FriendChatRoom?)> LoadFriendChatRoom(long roomId)
        {
            (ECode e, FriendChatRoomInfo? friendChatRoomInfo) = await this.QueryFriendChatRoomInfo(roomId);
            if (e != ECode.Success)
            {
                return (e, null);
            }

            if (friendChatRoomInfo == null)
            {
                return (ECode.RoomNotExist, null);
            }

            var room = new FriendChatRoom(friendChatRoomInfo);

            await this.server.roomLocationRedisW.WriteLocation(roomId, this.service.serviceId, this.service.sd.saveIntervalS + 60);

            this.AddRoomToDict(room);

            if (!room.saveTimer.IsAlive())
            {
                this.service.ss.SetSaveTimer(room);
            }

            this.service.ss.ClearDestroyTimer(room, RoomClearDestroyTimerReason.RoomLoginSuccess);

            return (ECode.Success, room);
        }

        void AddRoomToDict(Room room)
        {
            // runtime 初始化
            this.service.sd.AddRoom(room);

            room.OnAddedToDict();

            // qiucw
            // 这句会修改 roomInfo，必须放在 lastSceneRoomInfo.DeepCopyFrom 后面
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