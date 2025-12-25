namespace Script
{
    public class RoomManager_Shutdown : OnShutdown<RoomManagerService>
    {
        public RoomManager_Shutdown(Server server, RoomManagerService service) : base(server, service)
        {
        }

        protected override async Task StopBusinesses()
        {

        }
    }
}