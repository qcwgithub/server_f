using Data;

namespace Script
{
    public class Room_Start : OnStart<RoomService>
    {
        public Room_Start(Server server, RoomService service) : base(server, service)
        {
        }

        protected override async Task<ECode> Handle2()
        {
            await this.service.UpdateRuntimeInfo();

            return ECode.Success;
        }
    }
}