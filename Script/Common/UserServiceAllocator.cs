namespace Script
{
    public class UserServiceAllocator<S> : ServiceScript<S> where S : Service
    {
        public UserServiceAllocator(Server server, S service) : base(server, service)
        {
            
        }
    }
}