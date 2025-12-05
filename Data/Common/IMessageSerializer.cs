namespace Data
{
    public interface IMessageSerializer
    {
        byte[] Pack<T>(T msg);
        T Unpack<T>(ArraySegment<byte> msg);
    }
}