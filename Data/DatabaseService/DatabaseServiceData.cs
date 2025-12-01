using log4net;
using System.Collections.Generic;

namespace Data
{
    public class DatabaseServiceData : ServiceData
    {
        public static readonly List<ServiceType> s_connectToServiceIds = new List<ServiceType>
        {
            ServiceType.Global,
        };
        public LockControllerData lockControllerData;
        public ITimer timer_persistence_taskQueueHandler_Loop;
        public int persistence_lastAssignTaskQueueOwnersTimeS;
        public List<int> persistence_ownTaskQueues;
        public bool persistenceHandling;
        public DatabaseServiceData(ServiceTypeAndId serviceTypeAndId)
            : base(serviceTypeAndId, s_connectToServiceIds)
        {
            this.lockControllerData = new LockControllerData();
            this.persistence_lastAssignTaskQueueOwnersTimeS = 0;
            this.persistence_ownTaskQueues = new List<int>();
            this.persistenceHandling = false;
        }
    }
}