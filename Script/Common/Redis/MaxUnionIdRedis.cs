using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Data;
using System;

namespace Script
{
    public class MaxUnionIdRedis : MaxIdRedis
    {
        public override string WaitKey(int serverId) => GlobalKey.MaxUnionIdInitedFlag(serverId);
        public override string Key(int serverId) => GlobalKey.MaxUnionId(serverId);
    }
}