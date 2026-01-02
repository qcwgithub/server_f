namespace Data
{
    public class MessageContext
    {
        public IConnection connection;
        public User lockedUser;
        public long userLockedKey;
    }
}