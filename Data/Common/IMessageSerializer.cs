namespace Data
{
    public interface IMessageSerializer
    {
        byte[] Serialize<T>(T msg);
        T Deserialize<T>(ArraySegment<byte> msg);
    }
}