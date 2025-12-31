using System;
using System.Collections;
using System.Threading.Tasks;
using Data;
using MessagePack;

namespace Script
{
    public abstract class Handler<S, Msg, Res> : ServiceScript<S>, IHandler
        where S : Service
        where Msg : class
        where Res : class, new()
    {
        protected Handler(Server server, S service) : base(server, service)
        {
        }

        public log4net.ILog logger => this.service.logger;

        public abstract MsgType msgType { get; }

        public async Task<(ECode, object)> Handle(MessageContext context, object msg)
        {
            Res res = new Res();
            ECode e = await this.Handle(context, (Msg)msg, res);
            return (e, res);
        }

        public abstract Task<ECode> Handle(MessageContext context, Msg msg, Res res);
        public virtual (ECode, object) PostHandle(MessageContext context, object msg, ECode e, object res)
        {
            this.PostHandle(context, (Msg)msg, e, (Res)res);
            return (e, res);
        }

        public virtual void PostHandle(MessageContext context, Msg msg, ECode e, Res res)
        {

        }

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
    }
}