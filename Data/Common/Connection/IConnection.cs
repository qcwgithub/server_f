namespace Data
{
    public interface IConnection
    {
        int GetConnectionId();
        bool IsConnected();
        void SendBytes(MsgType msgType, byte[] msg, Action<ECode, ArraySegment<byte>>? cb, int? pTimeoutS);
    }
}