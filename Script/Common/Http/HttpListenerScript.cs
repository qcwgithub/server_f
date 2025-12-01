using Data;
using System.Net;

namespace Script
{
    public class HttpListenerScript : ServiceScript<Service>, IHttpListenerCallback
    {
        public void OnReceiveHttpRequest(HttpListenerContext context)
        {
            var msg = new MsgOnHttpRequest();
            msg.context = context;
            this.service.Dispatch(null, /* seq */0, MsgType._OnHttpRequest, msg, null);
        }

        public log4net.ILog GetLogger() => this.service.logger;
    }
}