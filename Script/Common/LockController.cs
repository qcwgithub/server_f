using Data;
using StackExchange.Redis;

namespace Script
{
    // 优点：
    // 加入了排队，一定会锁成功
    // 多个不冲突的请求，锁一次就好
    // 
    // 缺点：
    // 同时只有一个请求时，与 redis 通信的次数增多，等待也多
    public class LockController : ServiceScript<Service>
    {
        LockControllerData data;
        string takeLockControlKey;
        string lockedHashKey;
        string lockPrefix;
        public LockController(Server server, Service service, LockControllerData data, string takeLockControlKey, string lockedHashKey, string allowLockPrefix) : base(server, service)
        {
            this.data = data;
            this.takeLockControlKey = takeLockControlKey;
            this.lockedHashKey = lockedHashKey;
            this.lockPrefix = allowLockPrefix;
        }

        public IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        async Task<string> TakeControl()
        {
            string lockValue = System.Guid.NewGuid().ToString();
            // int c = 0;
            while (true)
            {
                bool success = await this.GetDb().StringSetAsync(this.takeLockControlKey, new RedisValue(lockValue),
                    TimeSpan.FromSeconds(10), When.NotExists);
                if (success)
                {
                    break;
                }

                await Task.Delay(10);
                // c++;
                // this.service.logger.InfoFormat("TakeControl try {0}", c);
            }

            return lockValue;
        }

        async Task ReleaseControl(string lockValue)
        {
            RedisValue redisValue = await this.GetDb().StringGetAsync(this.takeLockControlKey);
            if (redisValue == lockValue)
            {
                bool removed = await this.GetDb().KeyDeleteAsync(this.takeLockControlKey);

                // 这句就算了，因为如果下断点就会报
                // MyDebug.Assert(removed);
            }
        }

        void ConsumeRequests(HashSet<string> busys, out List<LockRequest> successList, out List<LockRequest> failedList)
        {
            // this.service.logger.Info("++++ requests.Count " + this.data.requests.Count);
            // this.service.logger.Info("++++ busys " + JsonUtils.stringify(busys));

            successList = null;
            failedList = null;

            List<LockRequest> requests = this.data.requests;
            for (int i = 0; i < requests.Count; i++)
            {
                LockRequest req = requests[i];
                bool ok = true;
                foreach (string key in req.keys)
                {
                    if (busys.Contains(key))
                    {
                        ok = false;
                        break;
                    }
                }

                if (ok)
                {
                    if (successList == null)
                    {
                        successList = new List<LockRequest>();
                    }
                    successList.Add(req);

                    // if (req.retry)
                    // {
                    // long time1 = DateTime.Now.Ticks / 10000;
                    // this.service.logger.InfoFormat("++++ SUC cost({0}) try({1}) {2}", time1 - req.time0, req.tryCount, JsonUtils.stringify(req.keys));
                    // }

                    busys.UnionWith(req.keys);
                    requests.RemoveAt(i);
                    i--;
                }
                else if (!req.retry)
                {
                    if (failedList == null)
                    {
                        failedList = new List<LockRequest>();
                    }
                    failedList.Add(req);

                    requests.RemoveAt(i);
                    i--;
                }
                else
                {
                    // req.tryCount++;
                    // this.service.logger.InfoFormat("++++ FAIL try({0}) {1}", req.tryCount, JsonUtils.stringify(req.keys));
                }
            }
        }

        async Task ApplyNewLockeds(List<LockRequest> successList)
        {
            long nowS = TimeUtils.GetTimeS();
            var okValues = new List<HashEntry>();
            foreach (LockRequest req in successList)
            {
                foreach (string key in req.keys)
                {
                    okValues.Add(new HashEntry(key, nowS + req.lockTimeS));
                }
            }
            await this.GetDb().HashSetAsync(this.lockedHashKey, okValues.ToArray());
        }

        async Task<HashSet<string>> GetBusys()
        {
            HashSet<string> busys = new HashSet<string>();
            HashEntry[] hashEntries = await this.GetDb().HashGetAllAsync(this.lockedHashKey);
            List<RedisValue> expires = null;
            long nowS = TimeUtils.GetTimeS();
            foreach (HashEntry entry in hashEntries)
            {
                if (nowS > RedisUtils.ParseInt(entry.Value))
                {
                    if (expires == null) expires = new List<RedisValue>();
                    expires.Add(entry.Name);
                }
                else
                {
                    busys.Add(entry.Name);
                }
            }
            if (expires != null)
            {
                this.service.logger.ErrorFormat("LockController: Lock expired, force unlock, key(s): {0}", JsonUtils.stringify(expires.Select(x => x.ToString()).ToArray()));
                await this.GetDb().HashDeleteAsync(this.lockedHashKey, expires.ToArray());
            }
            return busys;
        }

        async Task DidUnderControl()
        {
            HashSet<string> busys = await this.GetBusys();

            while (true)
            {
                this.ConsumeRequests(busys, out List<LockRequest> successList, out List<LockRequest> failedList);
                // this.service.logger.InfoFormat("++++ success {0} failed {1}", successList == null ? 0 : successList.Count, failedList == null ? 0 : failedList.Count);

                // int preCount = this.data.requests.Count; // 检测在 ApplyNewLockeds 期间，又往 requests 添加了东西
                if (successList != null)
                {
                    // 注意，这里不等了，减少单个请求的等待通信次数
                    this.ApplyNewLockeds(successList).Forget();
                }
                // int curCount = this.data.requests.Count;

                if (failedList != null)
                {
                    foreach (LockRequest req in failedList)
                    {
                        req.taskCompetitionSource.TrySetResult(false);
                    }
                }

                if (successList != null)
                {
                    foreach (LockRequest req in successList)
                    {
                        req.taskCompetitionSource.TrySetResult(true);
                    }
                }

                // if (curCount > preCount)
                // {
                // this.service.logger.InfoFormat("++++ Added requests while ApplyNewLockeds");
                // continue;
                // }

                break;
            }
        }

        async void LockProcedure()
        {
            this.data.procedureBusy = true;
            try
            {
                // int c = 0;
                while (true)
                {
                    string lockValue = await this.TakeControl();
                    await this.DidUnderControl(); // while
                    await this.ReleaseControl(lockValue); // 添加完一波就释放锁，不能一直霸占着

                    if (this.data.requests.Count > 0)
                    {
                        // 走到这里的话，只有可能是在 ReleaseControl 期间又添加
                        // c++;
                        // this.service.logger.Info("++++ TRY " + c);
                        await Task.Delay(10);
                        continue;
                    }

                    break;
                }
            }
            catch (Exception ex)
            {
                this.service.logger.Error("LockProcedure exception", ex);
            }
            finally
            {
                this.data.procedureBusy = false;
            }
        }

        public async Task<bool> Lock(string[] keys, int lockTimeS, bool retry)
        {
            foreach (string key in keys)
            {
                if (!key.StartsWith(this.lockPrefix))
                {
                    this.service.logger.ErrorFormat("LockController.Lock !key.StartsWith(this.lockPrefix), key {0} this.lockPrefix {1}", key, this.lockPrefix);
                }
            }

            var req = new LockRequest();
            req.keys = keys;
            req.lockTimeS = lockTimeS;
            req.retry = retry;
            req.taskCompetitionSource = new TaskCompletionSource<bool>();
            // req.time0 = DateTime.Now.Ticks / 10000;

            // this.service.logger.InfoFormat("++++ Lock {0} lockTimeS({1}) retry({2}) time0({3})", JsonUtils.stringify(keys), lockTimeS, retry, req.time0);

            this.data.requests.Add(req);

            if (this.data.requests.Count > 1000)
            {
                this.service.logger.ErrorFormat("requests.Count {0} > 1000", this.data.requests.Count);
            }

            if (!this.data.procedureBusy)
            {
                this.LockProcedure();
            }
            return await req.taskCompetitionSource.Task;
        }

        public async Task Unlock(string[] keys)
        {
            // this.service.logger.InfoFormat("++++ Unlock {0}", JsonUtils.stringify(keys));
            await this.GetDb().HashDeleteAsync(this.lockedHashKey, keys.Select(k => new RedisValue(k)).ToArray());
            if (!this.data.procedureBusy && this.data.requests.Count > 0)
            {
                this.LockProcedure();
            }
        }

        public void DetectLockTooLong(string msgType, int startS, int lockTimeS)
        {
            long useS = TimeUtils.GetTimeS() - startS;
            long per = useS * 100 / lockTimeS;
            if (per > 50)
            {
                this.service.logger.Error($"{msgType} lock too long {useS} / {lockTimeS} = {per}%");
            }
            // else if (useS != 0)
            // {
            //     this.service.logger.Info($"{msgType} lock time {useS} / {lockTimeS} = {per}%");
            // }
        }

        public void DetectLockTooLong(MsgType msgType, int startS, int lockTimeS)
        {
            this.DetectLockTooLong(msgType.ToString(), startS, lockTimeS);
        }
    }
}