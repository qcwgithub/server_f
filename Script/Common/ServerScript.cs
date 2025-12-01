using Data;

namespace Script
{
    public abstract class ServerScript
    {
        public Server server { get; set; }
    }

    public static class ServerScriptExt
    {
        public static SELF Init<SELF>(this SELF self, Server server)
            where SELF : ServerScript
        {
            self.server = server;
            return self;
        }
    }
}