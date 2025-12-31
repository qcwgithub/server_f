using Data;

namespace Script
{
    public partial class GatewayService
    {
        #region auto

        public async Task<MyResponse<ResGatewayDestroyUser>> DestroyUser(MsgGatewayDestroyUser msg)
        {
            return await this.dispatcher.Dispatch<MsgGatewayDestroyUser, ResGatewayDestroyUser>(default, MsgType._Gateway_DestroyUser, msg);
        }

        #endregion auto
    }
}