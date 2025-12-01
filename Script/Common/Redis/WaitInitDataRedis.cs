using Data;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Script
{
    // 等待某个 key 存在
    public abstract class WaitInitDataRedis<G> : ServerScript<G>
        where G : BaseServer
    {
        public abstract IDatabase GetDb();
        protected abstract string waitKey { get; }

        public async Task<ECode> WaitInit(BaseService service)
        {
            while (true)
            {
                bool exist = await this.GetDb().KeyExistsAsync(this.waitKey);
                if (exist)
                {
                    return ECode.Success;
                }

                service.logger.Error("WaitInitDataRedis.WaitInit should never get here");

                if (service.IsShuttingDown())
                {
                    return ECode.ServiceIsShuttingDown;
                }

                service.logger.InfoFormat("WaitInit '{0}'", this.waitKey);

                await Task.Delay(1000);
            }
        }
    }
}