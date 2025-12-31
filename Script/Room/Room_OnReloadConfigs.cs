using System;
using System.Linq;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Room_OnReloadConfigs : OnReloadConfigs<RoomService>
    {
        public Room_OnReloadConfigs(Server server, RoomService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgReloadConfigs msg, ResReloadConfigs res)
        {
            ECode e = await base.Handle(context, msg, res);
            if (e != ECode.Success)
            {
                return e;
            }

            // var sd = this.service.usData;
            // var msg = Utils.CastObject<MsgReloadConfigs>(_msg);

            return e;
        }
    }
}