namespace Data
{
    public class SocketServiceConnection : ServiceConnection
    {
        public readonly SocketConnection socketConnection;

        public SocketServiceConnection(ServiceType serviceType, int serviceId, ProtocolClientData socket, bool isConnector) : base(serviceType, serviceId)
        {
            this.socketConnection = new SocketConnection(socket, isConnector);
        }

        public override int GetConnectionId()
        {
            return this.socketConnection.GetConnectionId();            
        }

        public override void Connect()
        {
            this.socketConnection.Connect();
        }

        public override bool IsConnected()
        {
            return this.socketConnection.IsConnected();
        }

        public override bool IsConnecting()
        {
            return this.socketConnection.IsConnecting();
        }

        public override void SendBytes(MsgType msgType, byte[] msg, ReplyCallback? cb, int? pTimeoutS)
        {
            this.socketConnection.SendBytes(msgType, msg, cb, pTimeoutS);
        }

        public override void Close(string reason)
        {
            this.socketConnection.Close(reason);
        }

        public override bool IsClosed()
        {
            return this.socketConnection.IsClosed();
        }

        public override string? closeReason
        {
            get
            {
                return this.socketConnection.closeReason;
            }
        }
    }
}