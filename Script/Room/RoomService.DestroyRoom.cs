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
            ECode e;
            switch (room.roomType)
            {
                case RoomType.Scene:
                    e = await this.SaveSceneInfo((SceneRoom)room, "DestroyRoom");
                    break;

                case RoomType.Private:
                    e = await this.SaveFriendChatInfo((FriendChatRoom)room, "DestroyRoom");
                    break;

                default:
                    throw new Exception("Not handled RoomType." + room.roomType);
            }

            sd.RemoveRoom(room.roomId);
            this.CheckUpdateRuntimeInfo().Forget();
            return ECode.Success;
        }
    }
}