using Data;

namespace Script
{
    public class Gateway_OnConnectionClose : OnConnectionClose<GatewayService>
    {
        public Gateway_OnConnectionClose(Server server, GatewayService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(IConnection connection, MsgConnectionClose msg, ResConnectionClose res)
        {
            await base.Handle(connection, msg);
            
            if (connection is GatewayUserConnection userConnection)
            {
                GatewayUser? user = userConnection.GetUser();
                if (user == null)
                {
                    return ECode.Success;
                }

                this.service.logger.InfoFormat("{0} userId {1} closeReason {2}", this.msgType, user.userId, userConnection.closeReason);

                if (user.connection != null)
                {
                    user.connection.UnbindUser();
                    user.connection = null;
                }

                long nowS = TimeUtils.GetTimeS();
                user.offlineTimeS = nowS;

                if (!user.destroyTimer.IsAlive())
                {
                    this.service.ss.SetDestroyTimer(user);
                }
            }

            return ECode.Success;
        }
    }
}