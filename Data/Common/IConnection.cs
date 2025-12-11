namespace Data
{
    public interface IConnection
    {
        void Connect();
        bool IsConnecting();
        bool IsConnected();
        int GetConnectionId();
        bool IsClosed();
        void Close(string reason);
        void SendBytes(MsgType msgType, byte[] msg, Action<ECode, ArraySegment<byte>>? cb, int? pTimeoutS);
    }
}