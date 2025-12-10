using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Global_Start : OnStart<GlobalService>
    {
        public Global_Start(Server server, GlobalService service) : base(server, service)
        {
        }

        protected override async Task<ECode> Handle2()
        {
            return ECode.Success;
        }
    }
}