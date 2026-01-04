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
            this.service.OnHttpRequest(context);
        }

        public log4net.ILog GetLogger() => this.service.logger;
    }
}