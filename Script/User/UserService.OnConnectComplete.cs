using Data;

namespace Script
{
    public partial class UserService
    {
        public override async Task<ECode> OnConnectComplete(IConnection connection)
        {
            var e = await base.OnConnectComplete(connection);
            if (e != ECode.Success)
            {
                return e;
            }

            return e;
        }
    }
}