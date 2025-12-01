namespace Script
{
    public abstract class ServiceScript<S>
        where S : Service
    {
        public Server server { get; set; }
        public S service { get; set; }
    }

    public static class ServiceScriptExt
    {
        public static SELF Init<SELF, S>(this SELF self, S service)
            where SELF : ServiceScript<S>
            where S : Service
        {
            self.server = service.server;
            self.service = service;
            return self;
        }
    }
}