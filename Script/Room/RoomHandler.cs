using Data;

namespace Script
{
    public abstract class RoomHandler<Msg, Res> : Handler<RoomService, Msg, Res>
        where Msg : class
        where Res : class, new()
    {
        protected RoomHandler(Server server, RoomService service) : base(server, service)
        {
        }
    }
}