using System;
using System.Linq;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_OnReloadConfigs : OnReloadConfigs<UserService>
    {
        public override async Task<MyResponse> Handle(ProtocolClientData socket, MsgReloadConfigs msg)
        {
            MyResponse r = await base.Handle(socket, msg);
            if (r.err != ECode.Success)
            {
                return r;
            }

            // var sd = this.service.usData;
            // var msg = Utils.CastObject<MsgReloadConfigs>(_msg);

            return r;
        }
    }
}