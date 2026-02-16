using System.Threading.Tasks;
using Data;
using System.Diagnostics;
using System;


namespace Script
{
    public partial class DbService
    {
        //// AUTO CREATED ////
        async Task<(ECode, bool)> SaveUserBriefInfo(stDirtyElement element)
        {
            long userId = long.Parse(element.s1);
            MyDebug.Assert(userId > 0);

            UserBriefInfo info = await this.server.userBriefInfoProxy.OnlyForSave_GetFromRedis(userId);
            if (info == null)
            {
                this.logger.ErrorFormat("SaveUserBriefInfo {0} info==null", element);
                return (ECode.Error, false);
            }
            MyDebug.Assert(info.userId == userId);

            if (info is ICanBePlaceholder h && h.IsPlaceholder())
            {
                this.logger.ErrorFormat("SaveUserBriefInfo {0} info.IsPlaceholder()", element);
                return (ECode.Error, false);
            }

            ECode e = await this.collection_user_brief_info.Save(info);
            if (e != ECode.Success)
            {
                this.logger.ErrorFormat("SaveUserBriefInfo {0} error {1}", element, e);
                return (e, true);
            }

            return (ECode.Success, false);
        }
    }
}
