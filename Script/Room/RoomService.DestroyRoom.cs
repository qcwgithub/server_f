using Data;

namespace Script
{
    public partial class RoomService
    {
        public async Task<ECode> DestroyRoom(long roomId, RoomDestroyRoomReason reason)
        {
            this.logger.InfoFormat("DestroyRoom roomId {0}, reason {1}, preCount {2}", roomId, reason, this.sd.roomCount);

            Room? room = sd.GetRoom(roomId);
            if (room == null)
            {
                logger.InfoFormat("DestroyRoom room not exist, roomId: {0}", roomId);
                return ECode.RoomNotExist;
            }

            this.ss.ClearSaveTimer(room);

            room.destroying = true;

            // Save once
            ECode e = await this.SaveRoom(roomId, "DestroyRoom");
            if (e != ECode.Success)
            {
                return e;
            }

            sd.RemoveRoom(roomId);
            this.CheckUpdateRuntimeInfo().Forget();
            return ECode.Success;
        }
    }
}