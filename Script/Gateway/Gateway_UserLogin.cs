using System.Net;
using System.Net.Sockets;
using Data;

namespace Script
{
    public class Gateway_UserLogin : GatewayHandler<MsgUserLogin, ResUserLogin>
    {
        public Gateway_UserLogin(Server server, GatewayService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType.UserLogin;

        public override async Task<ECode> Handle(IConnection connection, MsgUserLogin msg, ResUserLogin res)
        {
            var clientConnection = (GatewayClientConnection)connection;

            if (msg.dict == null)
            {
                msg.dict = new Dictionary<string, string>();
            }

            (AddressFamily family, string ip) = this.GetIp(clientConnection.socket);
            msg.dict["$addressFamily"] = family.ToString();
            msg.dict["$ip"] = ip;

            return ECode.Success;
        }

        (AddressFamily, string) GetIp(ProtocolClientData socket)
        {
            EndPoint endPoint = socket.RemoteEndPoint;
            AddressFamily addressFamily = AddressFamily.Unknown;
            string ip = string.Empty;
            if (endPoint != null)
            {
                addressFamily = endPoint.AddressFamily;

                IPEndPoint ipEndPoint = endPoint as IPEndPoint;
                if (ipEndPoint != null)
                {
                    ip = ipEndPoint.Address.MapToIPv4().ToString();
                }
            }

            return (addressFamily, ip);
        }
    }
}