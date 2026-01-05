using Data;

namespace Script
{
    // 负责将 Redis 中需要落地的非个人数据写入到 MongoDB
    // 1) 数据发生变化时，DataProxy 调用 this.scriptEntry.saveTaskQueueRedis.RPushToTaskQueue(this.GetBelongTaskQueue(p1, p2), dirtyElement.ToString())
    //    将新的数据的 key （stDirtyElement）存入到对应的队列（taskQueue）中
    // 2) DBPlayer 定时将这个队列一个一个取出来保存到 MongoDB 中
    public partial class DbService
    {
        TaskQueueOwnersAssignmentInput? input;
        public async Task<ECode> Persistence(bool isShuttingDownSaveAll)
        {
            MyDebug.Assert(!this.sd.persistenceHandling);
            this.sd.persistenceHandling = true;

            try
            {

                if (isShuttingDownSaveAll)
                {
                    this.logger.InfoFormat("PersistenceScript isShuttingDownSaveAll = true");
                }

                //// 1 分配任务队列

                if (!isShuttingDownSaveAll)
                {
                    if (this.input == null)
                    {
                        this.input = new TaskQueueOwnersAssignmentInput();

                        this.input.get_lastAssignTaskQueueOwnersTimeS = () => this.sd.persistence_lastAssignTaskQueueOwnersTimeS;
                        this.input.set_lastAssignTaskQueueOwnersTimeS = v => this.sd.persistence_lastAssignTaskQueueOwnersTimeS = v;
                        this.input.QUEUES = PersistenceTaskQueueRedis.QUEUES;
                        this.input.sortQueues = PersistenceTaskQueueRedis.SortQueues;

                        this.input.assignTaskOwnersLockKey = DbKey.LockKey.AssignPersistenceTaskQueueOwners();
                        this.input.Lock = options => this.lockManuallyScript.Lock(this.lockController, options);
                        this.input.serviceType = this.data.serviceType;
                        this.input.serviceId = this.data.serviceId;
                        this.input.logger = this.logger;
                        this.input.who = "PersistenceScript";
                        this.input.Unlock = options => this.lockManuallyScript.Unlock(this.lockController, options, "PersistenceScript");
                    }

                    this.sd.persistence_ownTaskQueues = await TaskQueueOwnersAssignment.TryAssignTaskOwners(this.input, this.data.thisServerServiceConfigs, this.taskQueueOwnersRedis, this.sd.persistence_ownTaskQueues);
                }

                if (this.sd.persistence_ownTaskQueues.Count == 0)
                {
                    return ECode.Success;
                }

                //// 2 List -> SortedSet

                foreach (int taskQueue in this.sd.persistence_ownTaskQueues)
                {
                    while (true)
                    {
                        const int N = 10000;
                        int moved = await this.MoveFromListToSortedSet(taskQueue, N);
                        // this.logger.InfoFormat("PersistenceScript taskQueue {0} moved {1} to sortedset", taskQueue, dirtyElementWithTimes.Count);}
                        if (!isShuttingDownSaveAll || moved < N)
                        {
                            break;
                        }
                    }
                }

                //// 3

                var total = new Ptr<int>();
                var finished = new Ptr<int>();

                foreach (int taskQueue in this.sd.persistence_ownTaskQueues)
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
                    this.logger.InfoFormat("PersistenceScript isShuttingDownSaveAll = true finished {0} total {1}", finished.value, total.value);
                }

                return ECode.Success;
            }
            catch (Exception ex)
            {
                this.logger.Error("Persistence exception!", ex);
                return ECode.Exception;
            }
            finally
            {
                this.sd.persistenceHandling = false;
            }
        }

        async Task<int> MoveFromListToSortedSet(int taskQueue, int N)
        {
            List<stDirtyElementWithTime> dirtyElementWithTimes = await this.server.persistence_taskQueueRedis.LRangeOfTaskQueue(taskQueue, N);
            if (dirtyElementWithTimes.Count == 0)
            {
                return 0;
            }

            var de2time = new Dictionary<string, int>();
            foreach (stDirtyElementWithTime st in dirtyElementWithTimes)
            {
                de2time[st.dirtyElement] = st.timeS;
            }

            await this.server.persistence_taskQueueRedis.AddToSortedSetAndPopFromList(taskQueue, de2time, dirtyElementWithTimes.Count, this.logger);

            // this.logger.InfoFormat("PersistenceScript taskQueue {0} moved {1} to sortedset", taskQueue, dirtyElementWithTimes.Count);}
            return dirtyElementWithTimes.Count;
        }

        async Task<int> HandleTaskQueue(int taskQueue, Ptr<int> total, Ptr<int> finished, long? dueTimeS, int N)
        {
            List<string> dirtyElements = await this.server.persistence_taskQueueRedis.GetByDueTimeS(taskQueue, dueTimeS, N);
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

            await this.server.persistence_taskQueueRedis.RemoveFromSortedSet(taskQueue, okDirtyElements);
            return dirtyElements.Count;
        }

        void CheckHandleResult(Task task, List<string> removeFromSortedSets)
        {
            stPersistenceResult result = ((Task<stPersistenceResult>)task).Result;
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
        protected async Task<stPersistenceResult> HandleTask(int taskQueue, string dirtyElement)
        {
            // this.logger.InfoFormat("PersistenceScript taskQueue {0} save {1}", taskQueue, dirtyElement);

            ECode err;
            bool putBack = false;

            stDirtyElement element = stDirtyElement.FromString(dirtyElement);

            switch (element.e)
            {
                #region auto_callSave

                case DirtyElementType.AccountInfo:
                    (err, putBack) = await this.SaveAccountInfo(element);
                    break;


                #endregion auto_callSave

                default:
                    this.logger.ErrorFormat("PersistenceScript unknown element {0}", element.ToString());
                    err = ECode.Error;
                    putBack = true;
                    break;
            }

            var result = new stPersistenceResult();
            result.taskQueue = taskQueue;
            result.dirtyElement = dirtyElement;
            result.e = err;
            result.putBack = putBack;
            return result;
        }
    }
}