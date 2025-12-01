using System.Threading.Tasks;
using StackExchange.Redis;
using System;
using Data;

namespace Script
{
    public class WeChatRedis : ServerScript<GroupServer>
    {
        public IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// SuccessLoginResponse

        public async Task<Data.Wechat.SuccessLoginResponse> GetSuccessLoginResponse(string code)
        {
            RedisValue redisValue = await this.GetDb().StringGetAsync(WeChatKey.SuccessLoginResponse(code));
            if (redisValue.IsNullOrEmpty)
            {
                return null;
            }
            var r = JsonUtils.parse<Data.Wechat.SuccessLoginResponse>(redisValue);
            return r;
        }

        public async Task SaveSuccessLoginResponse(string code, Data.Wechat.SuccessLoginResponse r)
        {
            await this.GetDb().StringSetAsync(WeChatKey.SuccessLoginResponse(code), JsonUtils.stringify(r));
        }
        
        public async Task DeleteSuccessLoginResponse(string code)
        {
            await this.GetDb().KeyDeleteAsync(WeChatKey.SuccessLoginResponse(code));
        }

        //// access token

        public async Task<string> GetAccessToken()
        {
            return await this.GetDb().StringGetAsync(WeChatKey.AccessToken());
        }

        public async Task SaveAccessToken(string token, int aliveTimeS)
        {
            await this.GetDb().StringSetAsync(WeChatKey.AccessToken(), token, TimeSpan.FromSeconds(aliveTimeS));
        }

        //// session key

        public async Task<string> GetSessionKey(string openid)
        {
            return await this.GetDb().StringGetAsync(WeChatKey.SessionKey(openid));
        }

        public async Task SaveSessionKey(string openid, string session_key)
        {
            await this.GetDb().StringSetAsync(WeChatKey.SessionKey(openid), session_key);
        }
    }
}