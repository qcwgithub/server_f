using Data;

namespace Script
{
    public partial class RoomService
    {
        protected override async Task<ECode> Start2()
        {
            await this.UpdateRuntimeInfo();
            return ECode.Success;
        }
    }
}