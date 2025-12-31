using Data;

namespace Script
{
    public partial class CommandService
    {
        #region auto

        public async Task<MyResponse> PerformReloadScript(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Command_PerformReloadScript, msg);
        }
        public async Task<MyResponse> PerformSaveUserInfoToFile(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Command_PerformSaveUserInfoToFile, msg);
        }
        public async Task<MyResponse> PerformShowScriptVersion(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Command_PerformShowScriptVersion, msg);
        }
        public async Task<MyResponse> PerformGetPendingMsgList(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Command_PerformGetPendingMsgList, msg);
        }
        public async Task<MyResponse> PerformShutdown(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Command_PerformShutdown, msg);
        }
        public async Task<MyResponse> PerformPlayerGM(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Command_PerformPlayerGM, msg);
        }
        public async Task<MyResponse> PerformKick(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Command_PerformKick, msg);
        }
        public async Task<MyResponse> PerformSetPlayerGmFlag(MsgCommon msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Command_PerformSetPlayerGmFlag, msg);
        }

        #endregion auto
    }
}