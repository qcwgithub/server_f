namespace Data
{
    public interface IConnection
    {
        int GetConnectionId();
        bool IsConnected();
        void SendBytes(MsgType msgType, byte[] msg, ReplyCallback? cb, int? pTimeoutS);
    }
}