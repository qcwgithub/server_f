using Data;

namespace Script
{
    public class User_OnConnectionClose : OnConnectionClose<UserService>
    {
        public User_OnConnectionClose(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MsgContext context, MsgConnectionClose msg, ResConnectionClose res)
        {
            await base.Handle(context, msg);

            if (context.connection is ServiceConnection serviceConnection &&
                serviceConnection.serviceType == ServiceType.Gateway)
            {
                long nowS = TimeUtils.GetTimeS();
                foreach (var kv in this.service.sd.userDict)
                {
                    User user = kv.Value;
                    if (user.gatewayServiceId == serviceConnection.serviceId)
                    {
                        user.offlineTimeS = nowS;
                        this.service.ss.SetDestroyTimer(user, UserDestroyUserReason.DestroyTimer_GatewayDisconnect);
                    }
                }
            }

            return ECode.Success;
        }
    }
}