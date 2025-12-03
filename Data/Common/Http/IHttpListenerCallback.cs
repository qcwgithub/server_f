using System.Net;

namespace Data
{
    public interface IHttpListenerCallback
    {
        void OnReceiveHttpRequest(HttpListenerContext context);
        log4net.ILog GetLogger();
    }

    public interface IHttpListenerCallbackProvider
    {
        IHttpListenerCallback? GetHttpListenerCallback();
    }
}