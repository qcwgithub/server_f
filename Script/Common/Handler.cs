using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public abstract class Handler<S> : ServiceScript<S>, IHandler
        where S : Service
    {
        public log4net.ILog logger => this.service.logger;

        public abstract MsgType msgType { get; }
        public abstract Task<MyResponse> Handle(ProtocolClientData socket, object _msg);
        public virtual MyResponse PostHandle(ProtocolClientData socket, object _msg, MyResponse r) => r;

        protected async Task OnErrorExit(ECode e)
        {
            Console.WriteLine("{0} Error: {1}, Process is exiting", this.msgType, e);
            this.service.logger.FatalFormat("{0} Error: {1}, Process is exiting", this.msgType, e);
            await Task.Delay(1000);
            System.Environment.Exit(1);
        }

        /*
        protected async Task OnErrorExit(string message)
        {
            this.service.logger.ErrorFormat("{0} Error: {1}, Process is exiting", this.msgType, message);
            await Task.Delay(1000);
            System.Environment.Exit(1);
        }
        */

        protected async Task OnErrorExit(Exception ex)
        {
            this.service.logger.Fatal(string.Format("{0} exception, Process is exiting", this.msgType), ex);
            await Task.Delay(1000);
            System.Environment.Exit(1);
        }

        public ServerData serverData
        {
            get
            {
                return this.server.data;
            }
        }
    }
}