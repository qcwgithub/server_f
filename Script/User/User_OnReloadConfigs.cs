using System;
using System.Linq;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_OnReloadConfigs : OnReloadConfigs<UserService>
    {
        public User_OnReloadConfigs(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MsgContext context, MsgReloadConfigs msg, ResReloadConfigs res)
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