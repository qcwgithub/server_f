using Data;

namespace Script
{
    public partial class GatewayService
    {
        public void DestroyUser(GatewayUser user, GatewayDestroyUserReason reason, MsgKick? msgKick)
        {
            this.logger.InfoFormat("DestroyUser userId {0}, reason {1}, preCount {2}", user.userId, reason, this.sd.userCount);

            if (msgKick != null && user.connection != null && user.connection.IsConnected())
            {
                user.connection.Send(MsgType.Kick, msgKick, null, null);
            }

            this.sd.RemoveUser(user.userId);

            GatewayUserConnection? connection = user.connection;
            if (connection != null)
            {
                user.connection = null;
                connection.userId = 0;

                if (connection.IsConnected())
                {
                    connection.Close("Gateway_DestroyUser");
                }
            }
        }
    }
}