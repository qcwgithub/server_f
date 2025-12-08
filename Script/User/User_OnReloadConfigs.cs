using System;
using System.Linq;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_OnReloadConfigs : OnReloadConfigs<UserService>
    {
        public override async Task<ECode> Handle(ProtocolClientData socket, MsgReloadConfigs msg, ResReloadConfigs res)
        {
            ECode e = await base.Handle(socket, msg, res);
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