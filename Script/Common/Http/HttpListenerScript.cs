using Data;
using System.Net;

namespace Script
{
    public class HttpListenerScript : ServiceScript<Service>, IHttpListenerCallback
    {
        public HttpListenerScript(Server server, Service service) : base(server, service)
        {
        }

        public void OnReceiveHttpRequest(HttpListenerContext context)
        {
            var msg = new MsgOnHttpRequest();
            msg.context = context;
            this.service.dispatcher.Dispatch<MsgOnHttpRequest, ResOnHttpRequest>(this.service.data.localConnection, MsgType._OnHttpRequest, msg).Forget();
        }

        public log4net.ILog GetLogger() => this.service.logger;
    }
}