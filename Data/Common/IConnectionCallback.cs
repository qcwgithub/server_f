namespace Data
{
    public interface IConnectionCallback : IDataCallback
    {
        void OnConnectSuccess(IConnection connection);
        void OnMsg(IConnection connection, int seq, MsgType msgType, ArraySegment<byte> msg, ReplyCallback cb);
        void OnClose(IConnection connection);
    }
}