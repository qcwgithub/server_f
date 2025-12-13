using Data;

namespace Script
{
    public abstract class GatewayHandler<Msg, Res> : Handler<GatewayService, Msg, Res>
        where Msg : class
        where Res : class, new()
    {
        protected GatewayHandler(Server server, GatewayService service) : base(server, service)
        {
        }

        public GatewayServiceData sd { get { return this.service.sd; } }
    }
}