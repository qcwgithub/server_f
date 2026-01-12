using Data;

namespace Script
{
    public class ObjectLocationAssignment : ServiceScript<Service>
    {
        public readonly ObjectLocationAssignmentData assignmentData;
        public readonly ServiceRuntimeInfoRedisRW serviceRuntimeInfoRedis;
        private ObjectLocationAssignment(Server server, Service service, ObjectLocationAssignmentData assignmentData, string redisKey) : base(server, service)
        {
            this.assignmentData = assignmentData;
            this.serviceRuntimeInfoRedis = new ServiceRuntimeInfoRedisRW(server, redisKey);
        }

        public static ObjectLocationAssignment CreateUserLocationAssignment(Server server, Service service, ObjectLocationAssignmentData assignmentData)
        {
            return new ObjectLocationAssignment(server, service, assignmentData, CommonKey.UserServiceRuntimeInfos());
        }

        public static ObjectLocationAssignment CreateRoomLocationAssignment(Server server, Service service, ObjectLocationAssignmentData assignmentData)
        {
            return new ObjectLocationAssignment(server, service, assignmentData, CommonKey.RoomServiceRuntimeInfos());
        }

        public async Task<stObjectLocation> AssignLocation(long objectId)
        {
            long nowS = TimeUtils.GetTimeS();

            bool needUpdate = false;
            if (this.assignmentData.lastUpdateS == 0)
            {
                needUpdate = true;
            }
            else
            {
                if (nowS - this.assignmentData.lastUpdateS >= 60)
                {
                    needUpdate = true;
                }
            }
            if (needUpdate)
            {
                var dict = await this.serviceRuntimeInfoRedis.GetAll();
                nowS = TimeUtils.GetTimeS();
                this.assignmentData.Update(nowS, dict);
            }

            ServiceRuntimeInfo? selected = null;
            foreach (var kv in this.assignmentData.serviceRuntimeInfoDict)
            {
                ServiceRuntimeInfo info = kv.Value;
                if (!info.allowNew)
                {
                    continue;
                }

                if (!this.service.connectionCallbackScript.IsServiceConnected(info.serviceId))
                {
                    continue;
                }

                if (selected == null || info.busyCount < selected.busyCount)
                {
                    selected = info;
                }
            }

            int serviceId = 0;
            if (selected != null)
            {
                selected.busyCount++;
                serviceId = selected.serviceId;
            }

            string log = $"Alloc service id for objectId {objectId}, result {serviceId}";
            if (serviceId != 0)
            {
                this.service.logger.Info(log);
            }
            else
            {
                this.service.logger.Error(log);
            }

            return new stObjectLocation { serviceId = serviceId, expiry = nowS + 60 };
        }
    }
}