using Data;

namespace Script
{
    public interface IHandler
    {
        MsgType msgType { get; }
        Task<MyResponse> Handle(MessageContext context, object msg);
        void PostHandle(MessageContext context, object msg, MyResponse r);
    }
}