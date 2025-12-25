using System.Net;
using System.Net.WebSockets;

namespace Data
{
    public interface ISendClientMessageThroughGateway : IDataCallback
    {
        void SendClientMessageThroughGateway(ServiceConnection serviceConnection, long userId, MsgType msgType, byte[] msg, ReplyCallback reply, int? pTimeoutS);
    }

    public interface ISendClientMessageThroughGatewayProvider : IDataCallbackProvider
    {
        ISendClientMessageThroughGateway? GetSendClientMessageThroughGateway();
    }
}