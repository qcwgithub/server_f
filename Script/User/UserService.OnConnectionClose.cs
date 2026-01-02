using Data;

namespace Script
{
    public partial class UserService
    {
        public override async Task<ECode> OnConnectionClose(IConnection connection)
        {
            await base.OnConnectionClose(connection);

            if (connection is ServiceConnection serviceConnection &&
                serviceConnection.serviceType == ServiceType.Gateway)
            {
                long nowS = TimeUtils.GetTimeS();
                foreach (var kv in this.sd.userDict)
                {
                    User user = kv.Value;
                    if (user.gatewayServiceId == serviceConnection.serviceId)
                    {
                        user.offlineTimeS = nowS;
                        this.ss.SetDestroyTimer(user, UserDestroyUserReason.DestroyTimer_GatewayDisconnect);
                    }
                }
            }

            return ECode.Success;
        }
    }
}