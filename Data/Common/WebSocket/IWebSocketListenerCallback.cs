using System.Net;
using System.Net.WebSockets;

namespace Data
{
    public interface IWebSocketListenerCallback : IDataCallback
    {
        void OnReceiveWebSocketRequest(WebSocketListenerData webSocketListener, WebSocket webSocket);
        log4net.ILog GetLogger();
    }

    public interface IWebSocketListenerCallbackProvider : IDataCallbackProvider
    {
        IWebSocketListenerCallback? GetWebSocketListenerCallback();
    }
}