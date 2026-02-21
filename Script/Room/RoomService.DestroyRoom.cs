using Data;

namespace Script
{
    public partial class RoomService
    {
        public async Task<ECode> DestroyRoom(Room room, RoomDestroyRoomReason reason)
        {
            this.logger.InfoFormat("DestroyRoom roomId {0}, reason {1}, preCount {2}", room.roomId, reason, this.sd.roomCount);

            this.ss.ClearSaveTimer(room);

            // Save once
            ECode e = await this.SaveSceneInfo(room, "DestroyRoom");
            if (e != ECode.Success)
            {
                return e;
            }

            sd.RemoveRoom(room.roomId);
            this.CheckUpdateRuntimeInfo().Forget();
            return ECode.Success;
        }
    }
}