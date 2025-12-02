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

        public GlobalService(Server server, int serviceId) : base(server, serviceId)
        {
        }

        public GroupServiceConnectFromStatelessService connectFromStatelessService { get; private set; }
        public Global_OnReladConfigs global_OnReladConfigs { get; private set; }


        public collection_profile_normal_server_status collection_profile_normal_server_status;
        public collection_profile_config_manager collection_profile_config_manager;

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<GlobalService>();
            this.connectFromStatelessService = new GroupServiceConnectFromStatelessService(this);

            MongoRegister.Init();

            this.dispatcher.AddHandler(new Global_Start().Init(this));
            this.dispatcher.AddHandler(new Global_Shutdown().Init(this));

            this.dispatcher.AddHandler(new Global_GetServiceConfigs().Init(this.server, this));

            this.global_OnReladConfigs = new Global_OnReladConfigs().Init(this.server, this);
            this.dispatcher.AddHandler(this.global_OnReladConfigs, true);

            this.dispatcher.AddHandler(new ConfigManager_Tick_Loop().Init(this.server, this));
            this.dispatcher.AddHandler(new ConfigManager_Tick().Init(this.server, this));

            // manual
            this.collection_profile_normal_server_status = new collection_profile_normal_server_status().Init(this.server, this);
            this.collection_profile_config_manager = new collection_profile_config_manager().Init(this.server, this);
        }

        protected override Task<ResGetServiceConfigs> RequestServiceConfigs(string why)
        {
            throw new Exception();
        }

        public async Task<ResGetServiceConfigs> CreateResGetServiceConfigs()
        {
            var sd = this.globalServiceData;

            var res = new ResGetServiceConfigs();
            res.purpose = this.data.serverConfig.purpose;
            res.majorVersion = this.scriptEntry.GetScriptDllVersion().Major;
            res.minorVerson = this.scriptEntry.GetScriptDllVersion().Minor;
            res.open = await this.server.serverOpenRedis.IsOpen();

            res.groupServiceConfigs = sd.allGroupServiceConfigs;
            res.normalServiceConfigs = sd.allNormalServiceConfigs;
            res.normalServerStatusConfigs = sd.normalServerStatusConfigs;
            res.autoStartNewServer = sd.profileConfigManager.autoStartNewServer;

            return res;
        }

        public override async Task<ECode> InitServiceConfigsUntilSuccess()
        {
            await this.InitProfileConfigManager();

            ResGetServiceConfigs res = await this.CreateResGetServiceConfigs();

            if (!this.CheckResGetServiceConfigs(res, out ServiceConfig myServiceConfig, out string message))
            {
                throw new Exception(message);
            }

            this.data.SaveServiceConfigs(res);
            this.data.serviceConfig = myServiceConfig;

            return ECode.Success;
        }

        async Task InitProfileConfigManager()
        {
            var sd = this.globalServiceData;

            sd.profileConfigManager = await this.collection_profile_config_manager.Query_ProfileConfigManager_all();
            if (sd.profileConfigManager == null)
            {
                sd.profileConfigManager = new ProfileConfigManager();
                sd.profileConfigManager.autoStartNewServer = false; // !
                await this.collection_profile_config_manager.Save(sd.profileConfigManager);
            }
        }
    }
}