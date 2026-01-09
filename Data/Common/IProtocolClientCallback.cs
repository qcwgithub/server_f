namespace Data
{
    public interface IProtocolClientCallback : IDataCallback
    {
        IMessagePacker GetMessagePacker();

        void LogError(ProtocolClientData data, string str);
        void LogError(ProtocolClientData data, string str, Exception ex);
        void LogInfo(ProtocolClientData data, string str);

        void OnConnectComplete(ProtocolClientData data, bool success);

        void OnCloseComplete(ProtocolClientData data);
        void ReceiveFromNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msg, ReplyCallback cb);
    }
}