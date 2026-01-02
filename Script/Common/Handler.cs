using Data;

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

        public abstract MsgType msgType { get; }

        public async Task<MyResponse> Handle(MessageContext context, object msg)
        {
            Res res = new Res();
            ECode e = await this.Handle(context, (Msg)msg, res);
            return new MyResponse(e, res);
        }

        public abstract Task<ECode> Handle(MessageContext context, Msg msg, Res res);

        public virtual void PostHandle(MessageContext context, object msg, MyResponse r)
        {
            this.PostHandle(context, (Msg)msg, r.e, (Res)r.res);
        }

        public virtual void PostHandle(MessageContext context, Msg msg, ECode e, Res res)
        {

        }
    }
}