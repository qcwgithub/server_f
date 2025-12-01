namespace Data
{
    public interface IProtocolClientCallback
    {
        IMessagePacker GetMessagePacker(bool isMessagePack);

        int nextSocketId { get; }
        int nextMsgSeq { get; }

        void LogError(ProtocolClientData data, string str);
        void LogError(ProtocolClientData data, string str, Exception ex);
        void LogInfo(ProtocolClientData data, string str);

        void OnConnectComplete(ProtocolClientData data, bool success);

        void OnCloseComplete(ProtocolClientData data);
        void Dispatch(ProtocolClientData data, int seq, MsgType msgType, object msg, Action<ECode, object> cb);
    }

    public interface IProtocolClientCallbackProvider
    {
        IProtocolClientCallback GetProtocolClientCallback();
    }
}