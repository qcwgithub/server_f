namespace Data
{
    public interface ISendClientMessageThroughGateway : IDataCallback
    {
        void S_to_G(IServiceConnection serviceConnection, long userId, MsgType msgType, object msg, ReplyCallback? reply);
    }

    public interface ISendClientMessageThroughGatewayProvider : IDataCallbackProvider
    {
        ISendClientMessageThroughGateway? Get_S_to_G();
    }
}