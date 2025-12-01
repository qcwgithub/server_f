using System.Net;
using System;
using System.Net.WebSockets;
using System.Net.Http;
using System.Linq;

namespace Data
{
    public class WebSocketListenerData
    {
        public HttpListener httpListener;
        public IWebSocketListenerCallbackProvider callbackProvider;
        public IWebSocketListenerCallback callback => this.callbackProvider.GetWebSocketListenerCallback();
        public log4net.ILog logger => this.callback.GetLogger();

        public bool isForClient;
        public bool closed;
        public IPEndPoint remoteEndPoint;

        public void Listen(string[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
            {
                throw new Exception("WebSocketListenerData.Listen(): prefixes == null || prefixes.Length == 0");
            }

            this.httpListener = new HttpListener();
            foreach (string prefix in prefixes)
            {
                this.httpListener.Prefixes.Add(prefix);
            }
            this.httpListener.Start();
        }


#if DEBUG
        bool stopping;
        public async void Close()
#else
        public void Close()
#endif
        {
            if (this.closed)
            {
                // this.logError(this, $"call close on socketId({this.socketId}) with reason({reason}), but this.closed is true!");
                return;
            }
#if DEBUG
            this.stopping = true;
            using (var client = new HttpClient())
            {
                string prefix0 = this.httpListener.Prefixes.First().ToString();
                int index = prefix0.LastIndexOf(':');
                int count = 0;
                for (int i = index + 1; i < prefix0.Length; i++)
                {
                    if (char.IsDigit(prefix0[i]))
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                int port = int.Parse(prefix0.Substring(index + 1, count));
                await client.GetAsync("http://localhost:" + port);
            }
#endif
            // this.logInfo(this, $"call close on socketId({this.socketId}) with reason({reason})");
            this.closed = true;

            this.httpListener.Stop();
            this.httpListener = null;
            this.remoteEndPoint = null;
        }

        bool accepting = false;
        public async void StartAccept()
        {
            if (this.accepting)
            {
                MyDebug.Assert(false);
                return;
            }

            this.accepting = true;

            while (true)
            {
                HttpListenerContext context = null;

                try
                {
                    context = await this.httpListener.GetContextAsync();
#if DEBUG
                    if (this.stopping)
                    {
                        break;
                    }
#endif
                    if (!context.Request.IsWebSocketRequest)
                    {
                        this.logger.Info("!IsWebSocketRequest");
                        context.Response.StatusCode = 400;
                        continue;
                    }

                    HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                    WebSocket webSocket = webSocketContext.WebSocket;
                    // 获取远程IP地址
                    this.remoteEndPoint = context.Request.RemoteEndPoint;

                    this.callback.OnReceiveWebSocketRequest(this, webSocket);
                }
                catch (Exception ex)
                {
                    if (this.httpListener == null)
                    {
                        // 已经 Stop 了，不是错误
                        break;
                    }
                    else
                    {
                        this.logger.Error("this.httpListener.GetContextAsync exception: " + ex);
                        continue;
                    }
                }
            }
        }
    }
}