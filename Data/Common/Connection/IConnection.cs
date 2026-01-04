namespace Data
{
    public interface IConnection
    {
        int GetConnectionId();
        bool IsConnected();
        void Send(MsgType msgType, object msg, ReplyCallback? cb, int? pTimeoutS);
    }
}