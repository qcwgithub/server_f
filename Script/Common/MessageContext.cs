namespace Data
{
    public class MessageContext
    {
        public IConnection connection;
        public long msg_userId;

        // temp
        public string? lockValue;
    }
}