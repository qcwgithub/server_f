namespace Data
{
    public interface IProtocolClientCallback : IDataCallback
    {
        IMessagePacker GetMessagePacker();

        void LogError(string str);
        void LogError(string str, Exception ex);
        void LogInfo(string str);

        void OnConnectComplete(bool success);

        void OnCloseComplete(ProtocolClientData data);
        void ReceiveFromNetwork(int seq, MsgType msgType, ArraySegment<byte> msg, ReplyCallback cb);
    }
}