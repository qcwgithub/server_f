namespace Data
{
    public class UserConnection : IConnection
    {
        void IConnection.Close(string reason)
        {
            throw new NotImplementedException();
        }

        void IConnection.Connect()
        {
            throw new NotImplementedException();
        }

        int IConnection.GetConnectionId()
        {
            throw new NotImplementedException();
        }

        bool IConnection.IsClosed()
        {
            throw new NotImplementedException();
        }

        bool IConnection.IsConnected()
        {
            throw new NotImplementedException();
        }

        bool IConnection.IsConnecting()
        {
            throw new NotImplementedException();
        }

        public void BindUser(IConnection connection, User user)
        {
            if (!user.IsRealPrepareLogin(out MsgPrepareUserLogin? msgPreparePlayerLogin))
            {
                MyDebug.Assert(false);
            }

            user.connection = connection;
            connection.user = user;
            connection.userId = user.userId;
            connection.user_version = msgPreparePlayerLogin!.version;
            connection.lastUserId = user.userId;
        }

        public void UnbindUser(IConnection connection, User user)
        {
            user.connection = null;
            connection.user = null;
            connection.userId = 0;
            connection.user_version = string.Empty;
        }

        public object? GetUser(IConnection connetion)
        {
            return connetion.user == null ? null : connetion.user;
        }
    }
}