using MessagePack;

namespace Data
{
    public class BinaryMessageSerializer : IMessageSerializer
    {
        public byte[] Serialize<T>(T msg)
        {
            return MessagePackSerializer.Serialize<T>(msg);
        }

        public T Deserialize<T>(ArraySegment<byte> msg)
        {
            return MessagePackSerializer.Deserialize<T>(msg);
        }
    }
}