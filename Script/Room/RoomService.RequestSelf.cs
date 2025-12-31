using Data;

namespace Script
{
    public partial class RoomService
    {
        #region auto

        public async Task<MyResponse<ResRoomDestroyRoom>> DestroyRoom(MsgRoomDestroyRoom msg)
        {
            return await this.dispatcher.Dispatch<MsgRoomDestroyRoom, ResRoomDestroyRoom>(default, MsgType._Room_DestroyRoom, msg);
        }
        public async Task<MyResponse<ResSaveRoom>> SaveRoom(MsgSaveRoom msg)
        {
            return await this.dispatcher.Dispatch<MsgSaveRoom, ResSaveRoom>(default, MsgType._Room_SaveRoom, msg);
        }

        #endregion auto
    }
}