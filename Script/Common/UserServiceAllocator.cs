using Data;

namespace Script
{
    public class UserServiceAllocator<S> : ServiceScript<S> where S : Service
    {
        public readonly UserServiceAllocatorData allocatorData;
        public UserServiceAllocator(Server server, S service, UserServiceAllocatorData allocatorData) : base(server, service)
        {
            this.allocatorData = allocatorData;
        }

        public async Task<int> AllocUserServiceId(long userId)
        {
            bool needUpdate = false;
            if (this.allocatorData.lastUpdateS == 0)
            {
                needUpdate = true;
            }
            else
            {
                long nowS = TimeUtils.GetTimeS();
                if (nowS - this.allocatorData.lastUpdateS >= 60)
                {
                    needUpdate = true;
                }
            }
            if (needUpdate)
            {
                var dict = await this.server.userServiceInfoRedis.GetAll();
                long nowS = TimeUtils.GetTimeS();
                this.allocatorData.Update(nowS, dict);
            }

            UserServiceInfo? selected = null;
            foreach (var kv in this.allocatorData.userServiceInfoDict)
            {
                UserServiceInfo info = kv.Value;
                if (!info.allowNewUser)
                {
                    continue;
                }

                if (!this.service.protocolClientScriptForS.IsServiceConnected(info.serviceId))
                {
                    continue;
                }

                if (selected == null || info.userCount < selected.userCount)
                {
                    selected = info;
                }
            }
            
            int serviceId = 0;
            if (selected != null)
            {
                selected.userCount++;
                serviceId = selected.serviceId;
            }

            string log = $"Alloc user service id for userId {userId}, result {serviceId}";
            if (serviceId != 0)
            {
                this.service.logger.Info(log);
            }
            else
            {
                this.service.logger.Error(log);
            }

            return serviceId;
        }
    }
}