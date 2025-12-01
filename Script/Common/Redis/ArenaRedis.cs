using System;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Script
{
    public class ArenaRedis : ServerScript<NormalServer>
    {
        public IDatabase GetDb()
        {
            return this.server.serverData.redis_db;
        }
    }
}