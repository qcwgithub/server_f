using Data;

namespace Script
{
    public interface IHandler
    {
        MsgType msgType { get; }
        Task<(ECode, object)> Handle(MessageContext context, object msg);
        (ECode, object) PostHandle(MessageContext context, object msg, ECode e, object res);
    }
}