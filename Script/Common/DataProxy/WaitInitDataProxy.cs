using StackExchange.Redis;
using System.Threading.Tasks;

namespace Script
{
    // 等待某个 key 存在
    public abstract class WaitInitDataProxy<S> : ServiceScript<S>
        where S : Service
    {
        protected abstract IDatabase GetDb();
        protected abstract string waitKey { get; }

        public async Task WaitInit()
        {
            while (true)
            {
                bool exist = await this.GetDb().KeyExistsAsync(this.waitKey);
                if (exist)
                {
                    break;
                }

                this.service.logger.InfoFormat("WaitInit '{0}'", this.waitKey);

                await Task.Delay(1000);
            }
        }
    }
}