using System.Net;

namespace Data
{
    public interface IHttpListenerCallback : IDataCallback
    {
        void OnReceiveHttpRequest(HttpListenerContext context);
        log4net.ILog GetLogger();
    }

    public interface IHttpListenerCallbackProvider : IDataCallbackProvider
    {
        IHttpListenerCallback? GetHttpListenerCallback();
    }
}