using Data;

namespace Script
{
    public partial class UserService
    {
        #region auto

        public async Task<MyResponse> DestroyUser(MsgUserDestroyUser msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._User_DestroyUser, msg);
        }
        public async Task<MyResponse> SaveUser(MsgSaveUser msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._User_SaveUser, msg);
        }

        #endregion auto
    }
}