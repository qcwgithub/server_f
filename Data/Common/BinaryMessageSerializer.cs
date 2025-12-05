using MessagePack;

namespace Data
{
    public class BinaryMessageSerializer : IMessageSerializer
    {
        public byte[] Pack<T>(T msg)
        {
            return MessagePackSerializer.Serialize<T>(msg);
        }

        public T Unpack<T>(ArraySegment<byte> msg)
        {
            return MessagePackSerializer.Deserialize<T>(msg);
        }
    }
}