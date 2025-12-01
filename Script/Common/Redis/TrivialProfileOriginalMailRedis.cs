using Data;
using System.Collections.Generic;
using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Linq;
using longid = System.Int64;

namespace Script
{
    public class TrivialProfileOriginalMailRedis: ServerScript<GroupServer>
    {
        IDatabase GetDb()
        {
            return this.server.serverData.redis_db;
        }

        public string Key(longid mailId) => MailKey.TrivialOriginalMail(mailId);
        
        public async Task Save(ProfileOriginalMail originalMail)
        {
            MyDebug.Assert(originalMail.mailId > 0);
            MyDebug.Assert(originalMail.expireTimeS != ProfileMail.NERVER_EXPIRE);

            int nowS = TimeUtils.GetTimeS();
            // 这里不能是 Max(0, ...)，会异常
            await RedisUtils.SaveAsJson(this.GetDb(), this.Key(originalMail.mailId), originalMail, TimeSpan.FromSeconds(Math.Max(1, originalMail.expireTimeS - nowS)));
        }

        public async Task<ProfileOriginalMail> Get(longid mailId)
        {
            return await RedisUtils.GetFromJson<ProfileOriginalMail>(this.GetDb(), this.Key(mailId));
        }

        public async Task Delete(longid mailId)
        {
            await this.GetDb().KeyDeleteAsync(this.Key(mailId));
        }
    }
}