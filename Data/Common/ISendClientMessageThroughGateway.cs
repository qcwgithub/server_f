namespace Data
{
    public interface ISendClientMessageThroughGateway : IDataCallback
    {
        void S_to_G(ServiceConnection serviceConnection, long userId, MsgType msgType, object msg, ReplyCallback? reply, int? pTimeoutS);
    }

    public interface ISendClientMessageThroughGatewayProvider : IDataCallbackProvider
    {
        ISendClientMessageThroughGateway? Get_S_to_G();
    }
}