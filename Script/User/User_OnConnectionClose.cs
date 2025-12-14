using Data;

namespace Script
{
    public class User_OnConnectionClose : OnConnectionClose<UserService>
    {
        public User_OnConnectionClose(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(IConnection connection, MsgConnectionClose msg, ResConnectionClose res)
        {
            await base.Handle(connection, msg);

            // if (msg.isServer)
            // {
            //     return ECode.Success;
            // }

            if (connection is UserConnection userConnection)
            {
                User? user = userConnection.GetUser();
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