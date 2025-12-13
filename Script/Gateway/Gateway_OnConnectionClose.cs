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

            // if (msg.isServer)
            // {
            //     return ECode.Success;
            // }

            if (connection is GatewayUserConnection gatewayUserConnection)
            {
                User? user = gatewayUserConnection.GetUser();
                if (user == null)
                {
                    return ECode.Success;
                }

                this.service.logger.InfoFormat("{0} userId {1} closeReason {2}", this.msgType, user.userId, connection.closeReason);

                if (user.connection != null)
                {
                    user.connection.UnbindUser();
                    user.connection = null;
                }

                long nowS = TimeUtils.GetTimeS();
                user.offlineTimeS = nowS;

                // this.service.sqlLog.PlayerLogout(player);

                if (!user.destroyTimer.IsAlive())
                {
                    this.service.ss.SetDestroyTimer(user, this.msgType.ToString());
                }
            }

            return ECode.Success;
        }
    }
}