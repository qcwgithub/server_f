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
            
            if (connection is GatewayClientConnection clientConnection)
            {
                GatewayUser? user = clientConnection.GetUser();
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
            }

            return ECode.Success;
        }
    }
}