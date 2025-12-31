using Data;

namespace Script
{
    public partial class UserService
    {
        #region auto

        public async Task<MyResponse<ResUserDestroyUser>> DestroyUser(MsgUserDestroyUser msg)
        {
            return await this.dispatcher.Dispatch<MsgUserDestroyUser, ResUserDestroyUser>(default, MsgType._User_DestroyUser, msg);
        }
        public async Task<MyResponse<ResSaveUser>> SaveUser(MsgSaveUser msg)
        {
            return await this.dispatcher.Dispatch<MsgSaveUser, ResSaveUser>(default, MsgType._User_SaveUser, msg);
        }

        #endregion auto
    }
}