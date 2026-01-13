using System.Net.Sockets;

namespace Data
{
    public interface ITcpListenerCallback : IDataCallback
    {
        public class OnAcceptArg
        {
            public bool forClient;
            required public Socket socket;
        }
        void OnAccept(OnAcceptArg arg);
        void LogError(string str);
        void LogError(string str, Exception ex);
        void LogInfo(string str);
    }

    public interface ITcpListenerCallbackProvider : IDataCallbackProvider
    {
        ITcpListenerCallback GetTcpListenerCallback(TcpListenerData tcpListenerData);
    }
}