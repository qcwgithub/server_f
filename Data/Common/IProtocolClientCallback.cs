namespace Data
{
    // Called by socket thread
    // 实现者必须保证每一个函数都是线程安全
    public interface IProtocolClientCallback
    {
        ServerConfig.SocketSecurityConfig socketSecurityConfig { get; }
        void LogError(string str);
        void LogError(string str, Exception ex);
        void LogInfo(string str);

        void OnConnect(bool success);
        int OnReceive(byte[] buffer, int offset, int count);
        void OnClose();
    }
}