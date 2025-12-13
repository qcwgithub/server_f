using Data;

namespace Script
{
    // 负责将 Redis 中需要落地的非个人数据写入到 MongoDB
    // 1) 数据发生变化时，DataProxy 调用 this.scriptEntry.saveTaskQueueRedis.RPushToTaskQueue(this.GetBelongTaskQueue(p1, p2), dirtyElement.ToString())
    //    将新的数据的 key （stDirtyElement）存入到对应的队列（taskQueue）中
    // 2) DBPlayer 定时将这个队列一个一个取出来保存到 MongoDB 中
    public abstract class PersistenceTaskQueueHandler_Base<S> : LockManuallyHandler<S, MsgPersistence, ResPersistence>
        where S : Service
    {
        protected abstract string key_PersistenceTaskQueueOwners { get; }
        protected abstract bool persistenceHandling { get; set; }
        protected abstract long persistence_lastAssignTaskQueueOwnersTimeS { get; set; }
        protected abstract List<int> persistence_ownTaskQueues { get; set; }
        protected abstract string lockKey_AssignPersistenceTaskQueueOwners { get; }
        protected abstract PersistenceTaskQueueRedis taskQueueRedis { get; }

        TaskQueueOwnersRedis taskQueueOwnersRedis;
        public PersistenceTaskQueueHandler_Base(Server server, S service) : base(server, service)
        {
            this.taskQueueOwnersRedis = new TaskQueueOwnersRedis(server, this.key_PersistenceTaskQueueOwners);
        }

        TaskQueueOwnersAssignmentInput input;
        public override async Task<ECode> Handle(IConnection connection, MsgPersistence msg, ResPersistence res)
        {
            MyDebug.Assert(!this.persistenceHandling);
            this.persistenceHandling = true;

            bool isShuttingDownSaveAll = msg != null && msg.isShuttingDownSaveAll;
            if (isShuttingDownSaveAll)
            {
                this.service.logger.InfoFormat("{0} isShuttingDownSaveAll = true", this.msgType);
            }

            //// 1 分配任务队列

            if (!isShuttingDownSaveAll)
            {
                if (this.input == null)
                {
                    this.input = new TaskQueueOwnersAssignmentInput();

                    this.input.get_lastAssignTaskQueueOwnersTimeS = () => this.persistence_lastAssignTaskQueueOwnersTimeS;
                    this.input.set_lastAssignTaskQueueOwnersTimeS = v => this.persistence_lastAssignTaskQueueOwnersTimeS = v;
                    this.input.QUEUES = PersistenceTaskQueueRedis.QUEUES;
                    this.input.sortQueues = PersistenceTaskQueueRedis.SortQueues;

                    this.input.assignTaskOwnersLockKey = this.lockKey_AssignPersistenceTaskQueueOwners;
                    this.input.Lock = this.Lock;
                    this.input.serviceType = this.service.data.serviceType;
                    this.input.serviceId = this.service.data.serviceId;
                    this.input.logger = this.service.logger;
                    this.input.msgType = this.msgType;
                    this.input.Unlock = this.Unlock;
                }

                this.persistence_ownTaskQueues = await TaskQueueOwnersAssignment.TryAssignTaskOwners(this.input, this.service.data.thisServerServiceConfigs, this.taskQueueOwnersRedis, this.persistence_ownTaskQueues);
            }

            if (this.persistence_ownTaskQueues.Count == 0)
            {
                return ECode.Success;
            }

            //// 2 List -> SortedSet

            foreach (int taskQueue in this.persistence_ownTaskQueues)
            {
                while (true)
                {
                    const int N = 10000;
                    int moved = await this.MoveFromListToSortedSet(taskQueue, N);
                    // this.service.logger.InfoFormat("{0} taskQueue {1} moved {2} to sortedset", this.msgType, taskQueue, dirtyElementWithTimes.Count);}
                    if (!isShuttingDownSaveAll || moved < N)
                    {
                        break;
                    }
                }
            }

            //// 3

            var total = new Ptr<int>();
            var finished = new Ptr<int>();

            foreach (int taskQueue in this.persistence_ownTaskQueues)
            {
                while (true)
                {
                    const int N = TAKE_PER_QUEUE;

                    long? dueTimeS = null;
                    if (!isShuttingDownSaveAll)
                    {
                        dueTimeS = TimeUtils.GetTimeS() - 60;
                    }

                    int handled = await this.HandleTaskQueue(taskQueue, total, finished, dueTimeS, N);
                    if (!isShuttingDownSaveAll || handled < N)
                    {
                        break;
                    }
                }
            }

            MyDebug.Assert(finished.value == total.value);

            if (isShuttingDownSaveAll)
            {
                this.service.logger.InfoFormat("{0} isShuttingDownSaveAll = true finished {1} total {2}", this.msgType, finished.value, total.value);
            }

            return ECode.Success;
        }

        async Task<int> MoveFromListToSortedSet(int taskQueue, int N)
        {
            List<stDirtyElementWithTime> dirtyElementWithTimes = await this.taskQueueRedis.LRangeOfTaskQueue(taskQueue, N);
            if (dirtyElementWithTimes.Count == 0)
            {
                return 0;
            }

            var de2time = new Dictionary<string, int>();
            foreach (stDirtyElementWithTime st in dirtyElementWithTimes)
            {
                de2time[st.dirtyElement] = st.timeS;
            }

            await this.taskQueueRedis.AddToSortedSetAndPopFromList(taskQueue, de2time, dirtyElementWithTimes.Count, this.service.logger);

            // this.service.logger.InfoFormat("{0} taskQueue {1} moved {2} to sortedset", this.msgType, taskQueue, dirtyElementWithTimes.Count);}
            return dirtyElementWithTimes.Count;
        }

        async Task<int> HandleTaskQueue(int taskQueue, Ptr<int> total, Ptr<int> finished, long? dueTimeS, int N)
        {
            List<string> dirtyElements = await this.taskQueueRedis.GetByDueTimeS(taskQueue, dueTimeS, N);
            if (dirtyElements == null || dirtyElements.Count == 0)
            {
                return 0;
            }

            int index = 0;
            var tasks = new List<Task>();
            var okDirtyElements = new List<string>();

            while (true)
            {
                while (tasks.Count >= MAX_RUNNING_HALF)
                {
                    var compeletedTask = await Task.WhenAny(tasks);
                    tasks.Remove(compeletedTask);
                    finished.value++;
                    this.CheckHandleResult(compeletedTask, okDirtyElements);
                }

                int feedCount = this.FeedTasks(taskQueue, dirtyElements, ref index, tasks);
                total.value += feedCount;
                if (feedCount == 0)
                {
                    break;
                }
            }

            await Task.WhenAll(tasks);
            finished.value += tasks.Count;
            foreach (Task task in tasks)
            {
                this.CheckHandleResult(task, okDirtyElements);
            }

            await this.taskQueueRedis.RemoveFromSortedSet(taskQueue, okDirtyElements);
            return dirtyElements.Count;
        }

        public override void PostHandle(IConnection collection, MsgPersistence msg, ECode e, ResPersistence res)
        {
            base.PostHandle(collection, msg, e, res);

            this.persistenceHandling = false;
        }

        void CheckHandleResult(Task task, List<string> removeFromSortedSets)
        {
            stPersistenceResult result = (task as Task<stPersistenceResult>).Result;
            if (result.e != ECode.Success && result.putBack)
            {

            }
            else
            {
                removeFromSortedSets.Add(result.dirtyElement);
            }
        }

        const int MAX_RUNNING_HALF = 100;
        const int TAKE_PER_QUEUE = 1000;

        int FeedTasks(int taskQueue, List<string> dirtyElements, ref int index, List<Task> tasks)
        {
            int feedCount = 0;
            for (int i = 0; i < MAX_RUNNING_HALF; i++)
            {
                if (index < dirtyElements.Count)
                {
                    feedCount++;
                    tasks.Add(this.HandleTask(taskQueue, dirtyElements[index]));
                    index++;
                }
                else
                {
                    break;
                }
            }
            return feedCount;
        }
        protected abstract Task<stPersistenceResult> HandleTask(int taskQueue, string dirtyElement);
    }
}