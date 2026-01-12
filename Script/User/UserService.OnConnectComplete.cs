using Data;

namespace Script
{
    public partial class UserService
    {
        public override async Task<ECode> OnConnectComplete(IServiceConnection serviceConnection)
        {
            var e = await base.OnConnectComplete(serviceConnection);
            if (e != ECode.Success)
            {
                return e;
            }

            return e;
        }
    }
}