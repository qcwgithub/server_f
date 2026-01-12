using System.Net.Sockets;

namespace Data
{
    public class GatewayUserConnection : SocketConnection
    {
        public readonly GatewayUser user;

        public GatewayUserConnection(IConnectionCallbackProvider callbackProvider, Socket socket, bool forClient, GatewayUser user) : base(callbackProvider, socket, forClient, startRecv: false)
        {
            this.user = user;

            // 手动 StartRecv，否则会在基类构造函数里直接就收到消息
            this.StartRecv();
        }
    }
}