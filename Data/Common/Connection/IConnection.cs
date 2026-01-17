namespace Data
{
    public interface IConnection
    {
        bool IsConnected();
        void Send(MsgType msgType, object msg, ReplyCallback? cb);
    }
}