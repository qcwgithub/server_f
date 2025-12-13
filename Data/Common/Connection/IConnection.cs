namespace Data
{
    public interface IConnection
    {
        int GetConnectionId();
        void Connect();
        bool IsConnecting();
        bool IsConnected();
        void Close(string reason);
        bool IsClosed();
        string? closeReason { get; }
        void SendBytes(MsgType msgType, byte[] msg, Action<ECode, ArraySegment<byte>>? cb, int? pTimeoutS);
    }
}