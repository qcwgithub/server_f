using System;
using System.Collections.Generic;
using log4net;

namespace Data
{
    public sealed class UserServiceData : ServiceData
    {
        // playerId -> PlayerData
        public readonly Dictionary<long, User> userDict = new Dictionary<long, User>();
        public User? GetUser(long playerId)
        {
            User? user;
            return this.userDict.TryGetValue(playerId, out user) ? user : null;
        }

        public int destroyTimeoutS = 600;  // 下线后多久清除此玩家
        public int saveIntervalS = 60;
        public bool allowNewUser;

        //------------------------------------------------------

        public static readonly List<ServiceType> s_connectToServiceIds = new List<ServiceType>
        {
            ServiceType.Db,
            ServiceType.Global,
            ServiceType.Gateway,
        };

        public UserServiceData(ServiceTypeAndId serviceTypeAndId)
            : base(serviceTypeAndId, s_connectToServiceIds)
        {
            this.LoadConfigs();

            this.allowNewUser = true;
        }
        void LoadConfigs()
        {
            
        }

        public override void ReloadConfigs(bool all, List<string> files)
        {
            // this.configLoader.Reset_playerSpecialProfiles();
            if (all)
            {
                this.LoadConfigs();
            }
            else
            {
                
            }
        }

        public override void GetReloadConfigOptions(List<string> files)
        {
            files.Add("all");
        }

        public ITimer timer_tick_loop;
    }
}