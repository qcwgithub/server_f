using Data;

namespace Script
{
    public partial class RoomService
    {
        #region auto

        public async Task<MyResponse> DestroyRoom(MsgRoomDestroyRoom msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Room_DestroyRoom, msg);
        }
        public async Task<MyResponse> SaveRoom(MsgSaveRoom msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Room_SaveRoom, msg);
        }

        #endregion auto
    }
}