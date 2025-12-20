using System;
using System.Collections.Generic;
using log4net;

namespace Data
{
    public sealed class UserServiceData : ServiceData
    {
        public readonly Dictionary<long, User> userDict;
        public User? GetUser(long userId)
        {
            User? user;
            return this.userDict.TryGetValue(userId, out user) ? user : null;
        }

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
            this.userDict = new Dictionary<long, User>();

            this.LoadConfigs();

            this.allowNewUser = true;
        }
        void LoadConfigs()
        {
            
        }

        public override void ReloadConfigs(bool all, List<string> files)
        {
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