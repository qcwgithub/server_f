using Data;

namespace Script
{
    public interface IHandler
    {
        MsgType msgType { get; }
        object DeserializeMsg(ArraySegment<byte> msg);
        byte[] SerializeRes(object res);
        Task<(ECode, object)> Handle(ProtocolClientData socket, object msg);
        (ECode, object) PostHandle(ProtocolClientData socket, object msg, ECode e, object res);
    }
}