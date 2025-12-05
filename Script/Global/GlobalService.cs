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
            this.collection_profil_global = new collection_profile_global().Init(this.server, this);
            this.global_OnReladConfigs = new Global_OnReladConfigs().Init(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<GlobalService>();

            MongoRegister.Init();

            this.dispatcher.AddHandler(new Global_Start().Init(this.server, this));
            this.dispatcher.AddHandler(new Global_Shutdown().Init(this.server, this));
            this.dispatcher.AddHandler(new Global_GetServiceConfigs().Init(this.server, this));
            this.dispatcher.AddHandler(this.global_OnReladConfigs, true);
        }

        protected override Task<ResGetServiceConfigs> RequestServiceConfigs(string why)
        {
            throw new Exception();
        }

        public async Task<ResGetServiceConfigs> CreateResGetServiceConfigs()
        {
            var sd = this.globalServiceData;

            var res = new ResGetServiceConfigs();
            res.purpose = this.server.data.serverConfig.purpose;
            res.majorVersion = this.server.GetScriptDllVersion().Major;
            res.minorVerson = this.server.GetScriptDllVersion().Minor;

            res.allServiceConfigs = sd.allServiceConfigs;

            return res;
        }

        public override async Task<ECode> InitServiceConfigsUntilSuccess()
        {
            await this.InitProfileGlobal();

            ResGetServiceConfigs res = await this.CreateResGetServiceConfigs();

            if (!this.CheckResGetServiceConfigs(res, out ServiceConfig myServiceConfig, out string message))
            {
                throw new Exception(message);
            }

            this.data.SaveServiceConfigs(res);
            this.data.serviceConfig = myServiceConfig;

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