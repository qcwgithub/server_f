using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
namespace Script
{
    public class GlobalService : Service
    {
        public GlobalServiceData globalServiceData
        {
            get
            {
                return (GlobalServiceData)this.data;
            }
        }

        public readonly Global_OnReladConfigs global_OnReladConfigs;
        public readonly collection_profile_global collection_profil_global;

        public GlobalService(Server server, int serviceId) : base(server, serviceId)
        {
            this.collection_profil_global = new collection_profile_global(this.server, this);
            this.global_OnReladConfigs = new Global_OnReladConfigs(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<GlobalService>();

            MongoRegister.Init();

            this.dispatcher.AddHandler(new Global_Start(this.server, this));
            this.dispatcher.AddHandler(new Global_Shutdown(this.server, this));
            this.dispatcher.AddHandler(new Global_GetServiceConfigs(this.server, this));
            this.dispatcher.AddHandler(this.global_OnReladConfigs, true);
        }

        protected override Task<ResGetServiceConfigs?> RequestServiceConfigs(string why)
        {
            throw new Exception();
        }

        public void FillResGetServiceConfigs(ResGetServiceConfigs res)
        {
            var sd = this.globalServiceData;

            res.purpose = this.server.data.serverConfig.purpose;
            res.majorVersion = this.server.GetScriptDllVersion().Major;
            res.minorVerson = this.server.GetScriptDllVersion().Minor;

            res.allServiceConfigs = sd.allServiceConfigs;
        }

        public override async Task<ECode> InitServiceConfigsUntilSuccess()
        {
            await this.InitProfileGlobal();

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

        async Task InitProfileGlobal()
        {
            var sd = this.globalServiceData;

            sd.profileGlobal = await this.collection_profil_global.Query_ProfileGlobal_all();
            if (sd.profileGlobal == null)
            {
                sd.profileGlobal = new ProfileGlobal();
                await this.collection_profil_global.Save(sd.profileGlobal);
            }
        }
    }
}