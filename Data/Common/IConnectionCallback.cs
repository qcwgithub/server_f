namespace Data
{
    public interface IConnectionCallback : IDataCallback
    {
        ServerConfig.SocketSecurityConfig socketSecurityConfig { get; }
        IMessagePacker messagePacker { get; }
        void LogError(string str);
        void LogError(string str, Exception ex);
        void LogInfo(string str);
        int nextMsgSeq { get; }
        void OnConnect(IConnection connection, bool success);
        void OnMsg(IConnection connection, int seq, MsgType msgType, byte[] msgBytes, ReplyCallback? reply);
        void OnClose(IConnection connection);
    }

    public interface IConnectionCallbackProvider : IDataCallback
    {
        IConnectionCallback GetConnectionCallback();
    }
}