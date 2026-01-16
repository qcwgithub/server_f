using System.Net.Sockets;

namespace Data
{
    public class GatewayUserConnection : SocketConnection, IClientConnection
    {
        public long userId { get; set; }

        public GatewayUserConnection(IConnectionCallbackProvider callbackProvider, Socket socket) : base(callbackProvider, socket, true, startRecv: false)
        {
            // 手动 StartRecv，否则会在基类构造函数里直接就收到消息
            this.StartRecv();
        }
    }
}