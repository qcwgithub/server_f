namespace Script
{
    public class TaskQueueOwner
    {
        public int serviceId;

        // 下线超过 1 分钟，所管理的 taskQueue 会被其他人抢走
        public long offlineTimeS;
    }
}