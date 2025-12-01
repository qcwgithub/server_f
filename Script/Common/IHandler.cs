using Data;

namespace Script
{
    public interface IHandler
    {
        MsgType msgType { get; }
        Task<MyResponse> Handle(ProtocolClientData socket, object _msg);
        MyResponse PostHandle(ProtocolClientData socket, object msg, MyResponse r);
    }
}