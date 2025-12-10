using System;
using System.Net.Sockets;
using Data;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.WebSockets;

namespace Script
{
    public class WebSocketListenerScript : ServiceScript<Service>, IWebSocketListenerCallback
    {
        public WebSocketListenerScript(Server server, Service service) : base(server, service)
        {
        }


        public void OnReceiveWebSocketRequest(WebSocketListenerData webSocketListener, WebSocket webSocket)
        {
            // var msg = new MsgOnWebSocketRequest();
            // msg.context = context;
            // this.service.Dispatch(null, MsgType._OnWebSocketRequest, msg, null);

            // ((System.Net.WebSockets.ServerWebSocket)webSocket)
            // webSocket.tex
            //

            var webSocketClientData = new WebSocketClientData();
            webSocketClientData.AcceptorInit(this.service.data, webSocket, webSocketListener.isForClient, webSocketListener.remoteEndPoint);
        }

        public log4net.ILog GetLogger() => this.service.logger;
    }
}