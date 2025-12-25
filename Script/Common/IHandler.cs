using Data;

namespace Script
{
    public interface IHandler
    {
        MsgType msgType { get; }
        object DeserializeMsg(ArraySegment<byte> msg);
        ArraySegment<byte> SerializeRes(object res);
        Task<(ECode, object)> Handle(IConnection connection, object msg);
        (ECode, object) PostHandle(IConnection connection, object msg, ECode e, object res);
    }
}