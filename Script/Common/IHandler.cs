using Data;

namespace Script
{
    public interface IHandler
    {
        MsgType msgType { get; }
        object DeserializeMsg(ArraySegment<byte> msg);
        Task<MyResponse> Handle(ProtocolClientData socket, object msg);
        MyResponse PostHandle(ProtocolClientData socket, object msg, MyResponse r);
    }
}