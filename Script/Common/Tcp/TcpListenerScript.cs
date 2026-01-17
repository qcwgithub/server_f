using System.Collections.Concurrent;
using System.Net.Sockets;
using Data;

namespace Script
{
    public class TcpListenerScript : ServiceScript<Service>, ITcpListenerCallback
    {
        public TcpListenerScript(Server server, Service service) : base(server, service)
        {

        }

        // NOTE: Called by socket thread
        void ITcpListenerCallback.LogError(string str)
        {
            this.service.logger.Error(str);
        }

        // NOTE: Called by socket thread
        void ITcpListenerCallback.LogError(string str, Exception ex)
        {
            this.service.logger.Error(str, ex);
        }

        // NOTE: Called by socket thread
        void ITcpListenerCallback.LogInfo(string str)
        {
            this.service.logger.Info(str);
        }

        // NOTE: Called by socket thread
        void ITcpListenerCallback.OnAccept(ITcpListenerCallback.OnAcceptArg arg)
        {
            ET.ThreadSynchronizationContext.Instance.Post(this.OnAccept, arg);
        }

        // Called by main thread
        protected virtual void OnAccept(object? arg)
        {
            var acceptArg = (ITcpListenerCallback.OnAcceptArg)arg!;
            if (!acceptArg.forClient)
            {
                new SocketServiceConnection(this.service.data, acceptArg.socket);
            }
            else
            {
                MyDebug.Assert(false);
            }
        }
    }
}