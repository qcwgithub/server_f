namespace Data
{
    public class GatewayUserConnection : IConnection
    {
        public readonly SocketConnection socketConnection;
        public readonly GatewayUser user;

        public GatewayUserConnection(SocketConnection socketConnection, GatewayUser user)
        {
            this.socketConnection = socketConnection;
            this.user = user;
        }

        public bool IsConnected()
        {
            return this.socketConnection.IsConnected();
        }

        public void Send(MsgType msgType, object msg, ReplyCallback? cb, int? pTimeoutS)
        {
            this.socketConnection.Send(msgType, msg, cb, pTimeoutS);
        }
    }
}