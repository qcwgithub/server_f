namespace Data
{
    // 多线程调用
    public interface IProtocolClientCallback : IDataCallback
    {
        void LogError(string str);
        void LogError(string str, Exception ex);
        void LogInfo(string str);

        void OnConnect(bool success);
        int OnReceive(byte[] buffer, int offset, int count);
        void OnClose();
    }
}