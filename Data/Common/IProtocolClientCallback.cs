namespace Data
{
    // 多线程调用
    public interface IProtocolClientCallback : IDataCallback
    {
        IMessagePacker GetMessagePacker();

        void LogError(string str);
        void LogError(string str, Exception ex);
        void LogInfo(string str);

        void OnConnect(bool success);
        void OnMsg(int seq, MsgType msgType, ArraySegment<byte> msg, ReplyCallback cb);
        void OnClose();
    }
}