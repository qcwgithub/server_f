using Data;

namespace Script
{
    public class TaskQueueOwnersAssignment
    {
        public static async Task<List<int>> TryAssignTaskOwners(TaskQueueOwnersAssignmentInput input, List<ServiceConfig> thisServerServiceConfigs, TaskQueueOwnersRedis taskQueueOwnersRedis,
            List<int> ownTaskQueues)
        {
            if (input.get_lastAssignTaskQueueOwnersTimeS() == 0 ||
                TimeUtils.GetTimeS() - input.get_lastAssignTaskQueueOwnersTimeS() >= 60)
            {
                Dictionary<int, TaskQueueOwner> taskQueueManagers = await DoAssignTaskOwners(input, thisServerServiceConfigs, taskQueueOwnersRedis);

                // 不成功也保存时间
                input.set_lastAssignTaskQueueOwnersTimeS(TimeUtils.GetTimeS());

                if (taskQueueManagers == null)
                {
                    return null;
                }

                ownTaskQueues.Clear();
                foreach (var kv in taskQueueManagers)
                {
                    if (kv.Value.serviceId == input.serviceId)
                    {
                        ownTaskQueues.Add(kv.Key);
                    }
                }

                input.sortQueues(ownTaskQueues);

                // this.service.logger.InfoFormat("{0} final {1}", this.msgType, JsonUtils.stringify(this.handlerData.taskQueues));
            }

            return ownTaskQueues;
        }

        static async Task<Dictionary<int, TaskQueueOwner>> DoAssignTaskOwners(TaskQueueOwnersAssignmentInput input, List<ServiceConfig> thisServerServiceConfigs, TaskQueueOwnersRedis taskQueueOwnersRedis)
        {
            // this.service.logger.InfoFormat("AssignTaskManagers"); // 临时，需要删掉

            var options = new LockOptionsManually();

            // | Who                                      | Key
            // | Union_CompetitionDueTimeTaskQueueHandler | lock:union:unionCompetition:assignDueTimeTaskQueueOwners
            // | WorldMapDueTimeTaskQueueHandler          | lock:worldMap:assignDueTimeTaskQueueOwners
            // | PersistenceTaskQueueHandler              | lock:dbPlayer:assignPersistenceTaskQueueOwners
            // | DBGroup_PersistenceTaskQueueHandler      | lock:dbGroup:assignPersistenceTaskQueueOwners
            options.AddKey(input.assignTaskOwnersLockKey);
            options.lockTimeS = 60;
            options.retry = true;

            ECode e = await input.Lock(options);
            if (e != ECode.Success)
            {
                return null;
            }

            Task<Dictionary<int, TaskQueueOwner>> task2 = taskQueueOwnersRedis.GetTaskQueueOwners();
            await task2;

            // 非自己的同类
            List<ServiceConfig> others = thisServerServiceConfigs
                .Where(x => x.serviceType == input.serviceType && x.serviceId != input.serviceId)
                .ToList();

            Dictionary<int, TaskQueueOwner> taskQueueManagers = task2.Result;

            bool save = false;

            foreach (int queue in input.QUEUES)
            {
                if (!taskQueueManagers.ContainsKey(queue))
                {
                    save = true;
                    taskQueueManagers.Add(queue, new TaskQueueOwner());
                }
            }

            long nowS = TimeUtils.GetTimeS();

            // 自己：offlineTimeS -> 0
            // 别人：offlineTimeS -> >0
            foreach (var kv in taskQueueManagers)
            {
                int queue = kv.Key;
                TaskQueueOwner manager = kv.Value;
                if (manager.serviceId == 0)
                {
                    continue;
                }
                if (manager.serviceId == input.serviceId)
                {
                    if (manager.offlineTimeS > 0)
                    {
                        manager.offlineTimeS = 0;
                        save = true;
                        input.logger.InfoFormat("{0} Set my offlineTimeS to 0", input.who);
                    }
                    continue;
                }

                bool alive = others.Exists(_ => _.serviceId == manager.serviceId);
                if (alive)
                {
                    continue;
                }

                save = true;
                if (manager.offlineTimeS == 0)
                {
                    manager.offlineTimeS = nowS;
                }
                else if (nowS - manager.offlineTimeS >= 60)
                {
                    input.logger.WarnFormat("{0} Force-Release-Control serviceId {1} taskQueue {2}", input.who, manager.serviceId, queue);
                    manager.serviceId = 0;
                }
            }

            int total = others.Count + 1;
            int max = input.QUEUES.Length / total;
            int mod = input.QUEUES.Length % total;
            int maxer = total; // 最多几个人的数量是 max，例子：11 % 5 时，最多一个拥有 3
            if (mod != 0)
            {
                max++;
                maxer = mod;
            }

            // 自己：放弃、获取
            // 别人在线：不可以帮他放弃，因为他可能正在处理中

            var serviceId2count = new Dictionary<int, int>();
            foreach (var kv in taskQueueManagers)
            {
                int serviceId = kv.Value.serviceId;
                if (serviceId2count.TryGetValue(serviceId, out int count))
                {
                    serviceId2count[serviceId] = count + 1;
                }
                else
                {
                    serviceId2count[serviceId] = 1;
                }
            }

            int myCount;
            if (!serviceId2count.TryGetValue(input.serviceId, out myCount))
            {
                myCount = 0;
            }

            int curr_maxer = 0;
            foreach (var kv in serviceId2count)
            {
                if (kv.Value >= max)
                {
                    curr_maxer++;
                }
            }

            int myTarget = max;
            if (curr_maxer > maxer && myCount >= max)
            {
                myTarget--;
            }

            foreach (var kv in taskQueueManagers)
            {
                if (kv.Value.serviceId == input.serviceId && myCount > myTarget)
                {
                    myCount--;
                    save = true;
                    kv.Value.serviceId = 0;
                    kv.Value.offlineTimeS = 0;
                    input.logger.InfoFormat("{0} Self-Release-Control taskQueue {1}", input.who, kv.Key);
                }
                else if (kv.Value.serviceId == 0 && myCount < myTarget)
                {
                    myCount++;
                    save = true;
                    kv.Value.serviceId = input.serviceId;
                    kv.Value.offlineTimeS = 0;
                    input.logger.InfoFormat("{0} Take-Control taskQueue {1}", input.who, kv.Key);
                }
            }

            if (save)
            {
                save = false;
                await taskQueueOwnersRedis.SetTaskQueueOwners(taskQueueManagers);
            }

            await input.Unlock(options);

            // 
            return taskQueueManagers;
        }
    }
}