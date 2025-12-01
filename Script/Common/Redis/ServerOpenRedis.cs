using System.Threading.Tasks;
using StackExchange.Redis;

namespace Script
{
    public class ServerOpenRedis : ServerScript<GroupServer>
    {
        public IDatabase GetDb()
        {
            return this.server.serverData.redis_db;
        }

        string Key()
        {
            return GGlobalKey.ServerOpen(ScriptEntry.s_version.Major, ScriptEntry.s_version.Minor);
        }

        public async Task<bool> IsOpen()
        {
            string str = await this.GetDb().StringGetAsync(this.Key());
            return str == "1";
        }

        public async Task SetOpen(bool open)
        {
            if (open)
            {
                await this.GetDb().StringSetAsync(this.Key(), 1);
            }
            else
            {
                await this.GetDb().KeyDeleteAsync(this.Key());
            }
        }
    }
}