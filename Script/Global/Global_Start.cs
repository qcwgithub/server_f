using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Global_Start : OnStart<GlobalService>
    {
        protected override Task<ECode> Handle2()
        {
            return ECode.Success.ToTaskE();
        }
    }
}