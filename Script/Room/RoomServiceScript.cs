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

        public async Task<(ECode, RoomInfo?)> QueryRoomInfo(long roomId)
        {
            var msgDb = new MsgQuery_RoomInfo_by_roomId();
            msgDb.roomId = roomId;

            var r = await this.service.dbServiceProxy.Query_RoomInfo_by_roomId(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"QueryRoomInfo({roomId}) r.err {r.e}");
                return (r.e, null);
            }

            var resDb = r.CastRes<ResQuery_RoomInfo_by_roomId>();

            RoomInfo? roomInfo = resDb.result;
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

        public async Task<(ECode, Room?)> LoadRoom(long roomId)
        {
            (ECode e, RoomInfo? roomInfo) = await this.service.ss.QueryRoomInfo(roomId);
            if (e != ECode.Success)
            {
                return (e, null);
            }

            if (roomInfo == null)
            {
                return (ECode.RoomNotExist, null);
            }

            var room = new Room(roomInfo);

            await this.server.roomLocationRedisW.WriteLocation(roomId, this.service.serviceId, this.service.sd.saveIntervalS + 60);

            this.AddRoomToDict(room);

            if (!room.saveTimer.IsAlive())
            {
                this.service.ss.SetSaveTimer(room);
            }

            this.service.ss.ClearDestroyTimer(room, RoomClearDestroyTimerReason.RoomLoginSuccess);

            //
            List<ChatMessage> recents = await this.server.roomMessagesRedis.GetRecents(roomId, this.server.data.serverConfig.roomMessageConfig.initMessagesCount);
            this.service.logger.Info($"LoadRoom recent messages count {recents.Count}");
            foreach (ChatMessage message in recents)
            {
                this.service.sd.recentMessages.Enqueue(message);
            }

            return (ECode.Success, room);
        }

        void AddRoomToDict(Room room)
        {
            // runtime 初始化
            this.service.sd.AddRoom(room);

            // 有值就不能再赋值了，不然玩家上线下线就错了
            MyDebug.Assert(room.lastRoomInfo == null);

            room.lastRoomInfo = RoomInfo.Ensure(null);
            room.lastRoomInfo.DeepCopyFrom(room.roomInfo);

            // qiucw
            // 这句会修改 roomInfo，必须放在 lastRoomInfo.DeepCopyFrom 后面
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