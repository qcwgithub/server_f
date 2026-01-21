using Data;

namespace Script
{
    public partial class UserService
    {
        public override async Task<ECode> OnConnectionClose(IConnection connection)
        {
            await base.OnConnectionClose(connection);

            if (connection is IServiceConnection serviceConnection &&
                serviceConnection.knownWho &&
                !serviceConnection.isCommand &&
                serviceConnection.serviceType == ServiceType.Gateway)
            {
                long nowS = TimeUtils.GetTimeS();
                foreach (var kv in this.sd.userDict)
                {
                    User user = kv.Value;
                    if (user.connection != null && user.connection.gatewayServiceId == serviceConnection.serviceId)
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