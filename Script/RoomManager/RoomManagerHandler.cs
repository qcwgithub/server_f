using Data;

namespace Script
{
    public abstract class RoomManagerHandler<Msg, Res> : Handler<RoomManagerService, Msg, Res>
        where Msg : class
        where Res : class, new()
    {
        protected RoomManagerHandler(Server server, RoomManagerService service) : base(server, service)
        {
        }
    }
}