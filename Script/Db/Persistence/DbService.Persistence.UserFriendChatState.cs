using System.Threading.Tasks;
using Data;
using System.Diagnostics;
using System;


namespace Script
{
    public partial class DbService
    {
        //// AUTO CREATED ////
        async Task<(ECode, bool)> SaveUserFriendChatState(stDirtyElement element)
        {
            long userId = long.Parse(element.s1);
            MyDebug.Assert(userId > 0);

            UserFriendChatState info = await this.server.userFriendChatStateProxy.OnlyForSave_GetFromRedis(userId);
            if (info == null)
            {
                this.logger.ErrorFormat("SaveUserFriendChatState {0} info==null", element);
                return (ECode.Error, false);
            }
            MyDebug.Assert(info.userId == userId);

            if (info is ICanBePlaceholder h && h.IsPlaceholder())
            {
                this.logger.ErrorFormat("SaveUserFriendChatState {0} info.IsPlaceholder()", element);
                return (ECode.Error, false);
            }

            ECode e = await this.collection_user_friend_chat_state.Save(info);
            if (e != ECode.Success)
            {
                this.logger.ErrorFormat("SaveUserFriendChatState {0} error {1}", element, e);
                return (e, true);
            }

            return (ECode.Success, false);
        }
    }
}
