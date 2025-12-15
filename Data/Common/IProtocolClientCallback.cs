namespace Data
{
    public interface IProtocolClientCallback : IDataCallback
    {
        IMessagePacker GetMessagePacker();

        int nextSocketId { get; }
        int nextMsgSeq { get; }

        void LogError(ProtocolClientData data, string str);
        void LogError(ProtocolClientData data, string str, Exception ex);
        void LogInfo(ProtocolClientData data, string str);

        void OnConnectComplete(ProtocolClientData data, bool success);

        void OnCloseComplete(ProtocolClientData data);
        void DispatchNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msg, Action<ECode, byte[]>? cb);
    }

    public interface IProtocolClientCallbackProvider : IDataCallbackProvider
    {
        IProtocolClientCallback? GetProtocolClientCallback(ProtocolClientData protocolClientData);
    }
}