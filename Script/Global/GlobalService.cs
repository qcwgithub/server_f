using Data;

namespace Script
{
    public partial class GlobalService : Service
    {
        public GlobalServiceData sd
        {
            get
            {
                return (GlobalServiceData)this.data;
            }
        }

        public readonly Global_OnReladConfigs global_OnReladConfigs;

        public GlobalService(Server server, int serviceId) : base(server, serviceId)
        {
            this.global_OnReladConfigs = new Global_OnReladConfigs(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<GlobalService>();

            MongoRegister.Init();
        }

        protected override Task<ResGetServiceConfigs?> RequestServiceConfigs(string why)
        {
            throw new Exception();
        }

        public void FillResGetServiceConfigs(ResGetServiceConfigs res)
        {
            var sd = this.sd;

            res.purpose = this.server.data.serverConfig.purpose;
            res.majorVersion = this.server.GetScriptDllVersion().Major;
            res.minorVerson = this.server.GetScriptDllVersion().Minor;

            res.allServiceConfigs = sd.allServiceConfigs;
        }

        public override async Task<ECode> InitServiceConfigsUntilSuccess()
        {
            var res = new ResGetServiceConfigs();
            this.FillResGetServiceConfigs(res);

            if (!this.CheckResGetServiceConfigs(res, out ServiceConfig? myServiceConfig, out string message))
            {
                throw new Exception(message);
            }

            this.data.SaveServiceConfigs(res);
            this.data.serviceConfig = myServiceConfig!;

            return ECode.Success;
        }
    }
}