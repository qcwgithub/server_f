namespace Data
{
    public interface ISendClientMessageThroughGateway : IDataCallback
    {
        void SendClientMessageThroughGateway(ServiceConnection serviceConnection, long userId, MsgType msgType, object msg, ReplyCallback reply, int? pTimeoutS);
    }

    public interface ISendClientMessageThroughGatewayProvider : IDataCallbackProvider
    {
        ISendClientMessageThroughGateway? GetSendClientMessageThroughGateway();
    }
}