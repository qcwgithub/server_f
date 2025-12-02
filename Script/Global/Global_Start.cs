using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Global_Start : OnStart<GlobalService>
    {
        protected override Task<ECode> Handle2()
        {
            var sd = this.service.globalServiceData;
            sd.timer_tick_Loop = this.server.timerScript.SetTimer(this.service.serviceId, 0, MsgType._ConfigManager_Tick_Loop, null);
            return ECode.Success.ToTaskE();
        }
    }
}