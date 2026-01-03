namespace Data
{
    public class RobotClientConnection : SocketConnection
    {
        public RobotClientConnection(ProtocolClientData socket, bool isConnector) : base(socket, isConnector)
        {
        }
    }
}