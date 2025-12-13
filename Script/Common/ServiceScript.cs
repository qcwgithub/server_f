namespace Script
{
    public abstract class ServiceScript<S> where S : Service
    {
        public readonly Server server;
        public readonly S service;

        public ServiceScript(Server server, S service)
        {
            this.server = server;
            this.service = service;
        }
    }
}