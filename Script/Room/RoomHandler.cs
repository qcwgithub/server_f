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


        public RoomServiceData sd { get { return this.service.sd; } }

        public RoomServiceScript usScript { get { return this.service.ss; } }
    }
}