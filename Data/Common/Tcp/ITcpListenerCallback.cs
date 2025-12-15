using System.Net.Sockets;

namespace Data
{
    public interface ITcpListenerCallback : IDataCallback
    {
        void OnAcceptComplete(TcpListenerData data, SocketAsyncEventArgs e);
        void LogError(string str);
        void LogInfo(string str);
    }

    public interface ITcpListenerCallbackProvider : IDataCallbackProvider
    {
        ITcpListenerCallback? GetTcpListenerCallback(TcpListenerData tcpListenerData);
    }
}