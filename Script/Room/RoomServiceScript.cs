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

        // todo duplicate
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

            var msg = new MsgSaveRoom { roomId = room.roomId, reason = "SetSaveTimer" };
            room.saveTimer = server.timerScript.SetLoopTimer(this.service.serviceId, SEC, MsgType._Room_SaveRoom, msg);
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
                SEC, MsgType._Room_DestroyRoom,
                new MsgRoomDestroyRoom { roomId = room.roomId, reason = reason });
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