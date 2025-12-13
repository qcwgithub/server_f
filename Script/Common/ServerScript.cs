using Data;

namespace Script
{
    public abstract class ServerScript
    {
        public readonly Server server;

        public ServerScript(Server server)
        {
            this.server = server;
        }
    }
}