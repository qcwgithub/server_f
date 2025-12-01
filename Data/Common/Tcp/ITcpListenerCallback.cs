using System;
using System.Net.Sockets;
using System.IO;

namespace Data
{
    public interface ITcpListenerCallback
    {
        // void onListenerComplete(TcpListenerData data, SocketAsyncEventArgs e);
        void OnAcceptComplete(TcpListenerData data, SocketAsyncEventArgs e);
        void LogError(string str);
        void LogInfo(string str);
    }

    public interface ITcpListenerCallbackProvider
    {
        ITcpListenerCallback GetTcpListenerCallback();
    }
}