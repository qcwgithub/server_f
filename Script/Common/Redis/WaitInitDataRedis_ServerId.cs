using Data;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace Script
{
    // 等待某个 key 存在
    public abstract class WaitInitDataRedis_ServerId<G> : ServerScript<G>
        where G : NormalServer
    {
        public abstract IDatabase GetDb();
        public abstract string WaitKey(int serverId);

        public async Task<ECode> WaitInit(BaseService service, ServerInfo serverInfo)
        {
            if (serverInfo == null || serverInfo.serverIdInfoDict.Count == 0)
            {
                return ECode.Success;
            }

            RedisKey[] redisKeys = serverInfo.serverIdInfoDict.Select(kv =>
            {
                int serverId = kv.Key;
                string key_s = this.WaitKey(serverId);
                MyDebug.Assert(key_s.EndsWith("Inited")); // 防止写错覆盖了 key
                return new RedisKey(key_s);
            }).ToArray();

            while (true)
            {
                long count = await this.GetDb().KeyExistsAsync(redisKeys);
                if (count == redisKeys.Length)
                {
                    return ECode.Success;
                }

                service.logger.Error("WaitInitDataRedis_ServerId.WaitInit should never get here");

                if (service.IsShuttingDown())
                {
                    return ECode.ServiceIsShuttingDown;
                }

                service.logger.Info($"{this.GetType().Name} WaitInit...");

                await Task.Delay(1000);
            }
        }
    }
}