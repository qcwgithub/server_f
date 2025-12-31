using Data;

namespace Script
{
    public partial class GatewayService
    {
        #region auto

        public async Task<MyResponse> DestroyUser(MsgGatewayDestroyUser msg)
        {
            return await this.dispatcher.Dispatch(default, MsgType._Gateway_DestroyUser, msg);
        }

        #endregion auto
    }
}