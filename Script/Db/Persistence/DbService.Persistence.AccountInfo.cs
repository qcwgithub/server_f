using System.Threading.Tasks;
using Data;
using System.Diagnostics;
using System;


namespace Script
{
    public partial class DbService
    {
        //// AUTO CREATED ////
        async Task<(ECode, bool)> SaveAccountInfo(stDirtyElement element)
        {
            string channel = element.s1;
            MyDebug.Assert(!string.IsNullOrEmpty(channel));
            string channelUserId = element.s2;
            MyDebug.Assert(!string.IsNullOrEmpty(channelUserId));

            AccountInfo info = await this.server.accountInfoProxy.OnlyForSave_GetFromRedis(channel, channelUserId);
            if (info == null)
            {
                this.logger.ErrorFormat("SaveAccountInfo {0} info==null", element);
                return (ECode.Error, false);
            }
            MyDebug.Assert(info.channel == channel);
            MyDebug.Assert(info.channelUserId == channelUserId);

            if (info is ICanBePlaceholder h && h.IsPlaceholder())
            {
                this.logger.ErrorFormat("SaveAccountInfo {0} info.IsPlaceholder()", element);
                return (ECode.Error, false);
            }

            ECode e = await this.collection_account_info.Save(info);
            if (e != ECode.Success)
            {
                this.logger.ErrorFormat("SaveAccountInfo {0} error {1}", element, e);
                return (e, true);
            }

            return (ECode.Success, false);
        }
    }
}
