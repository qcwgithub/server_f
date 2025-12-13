namespace Data
{
    public class TaskQueueOwnersAssignmentInput
    {
        public Func<long> get_lastAssignTaskQueueOwnersTimeS;
        public Action<long> set_lastAssignTaskQueueOwnersTimeS;
        public int[] QUEUES;
        public Action<List<int>> sortQueues;

        public string assignTaskOwnersLockKey;
        public Func<LockOptionsManually, Task<ECode>> Lock;
        public ServiceType serviceType;
        public int serviceId;
        public log4net.ILog logger;
        public MsgType msgType;
        public Func<LockOptionsManually, Task> Unlock;
    }
}