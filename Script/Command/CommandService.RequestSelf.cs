using Data;

namespace Script
{
    public partial class CommandService
    {
        #region auto

        public async Task<MyResponse<ResCommon>> PerformReloadScript(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch<MsgCommon, ResCommon>(default, MsgType._Command_PerformReloadScript, msg);
        }
        public async Task<MyResponse<ResCommon>> PerformSaveUserInfoToFile(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch<MsgCommon, ResCommon>(default, MsgType._Command_PerformSaveUserInfoToFile, msg);
        }
        public async Task<MyResponse<ResCommon>> PerformShowScriptVersion(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch<MsgCommon, ResCommon>(default, MsgType._Command_PerformShowScriptVersion, msg);
        }
        public async Task<MyResponse<ResCommon>> PerformGetPendingMsgList(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch<MsgCommon, ResCommon>(default, MsgType._Command_PerformGetPendingMsgList, msg);
        }
        public async Task<MyResponse<ResCommon>> PerformShutdown(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch<MsgCommon, ResCommon>(default, MsgType._Command_PerformShutdown, msg);
        }
        public async Task<MyResponse<ResCommon>> PerformPlayerGM(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch<MsgCommon, ResCommon>(default, MsgType._Command_PerformPlayerGM, msg);
        }
        public async Task<MyResponse<ResCommon>> PerformKick(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch<MsgCommon, ResCommon>(default, MsgType._Command_PerformKick, msg);
        }
        public async Task<MyResponse<ResCommon>> PerformSetPlayerGmFlag(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch<MsgCommon, ResCommon>(default, MsgType._Command_PerformSetPlayerGmFlag, msg);
        }

        #endregion auto
    }
}