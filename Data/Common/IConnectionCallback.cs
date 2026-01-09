namespace Data
{
    public interface IConnectionCallback : IDataCallback
    {
        void OnConnectComplete(IConnection connection, bool success);
        void OnMsg(IConnection connection, int seq, MsgType msgType, ArraySegment<byte> msg, ReplyCallback cb);
        void OnCloseComplete(IConnection connection);
    }
}