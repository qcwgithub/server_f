using System.Net;
using System.Net.WebSockets;

namespace Data
{
    public interface IWebSocketListenerCallback
    {
        void OnReceiveWebSocketRequest(WebSocketListenerData webSocketListener, WebSocket webSocket);
        log4net.ILog GetLogger();
    }

    public interface IWebSocketListenerCallbackProvider
    {
        IWebSocketListenerCallback GetWebSocketListenerCallback();
    }
}