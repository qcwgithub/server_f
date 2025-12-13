using Data;

namespace Script
{
    public abstract class SnowflakeScript<S> : ServiceScript<S> where S : Service
    {
        public readonly SnowflakeData snowflakeData;
        public SnowflakeScript(Server server, S service, SnowflakeData snowflakeData) : base(server, service)
        {
            this.snowflakeData = snowflakeData;
        }

        public static readonly long MAX_INC = (1 << 10) - 1;

        // 1 41 13 10
        public static long Encode(long stamp, long workerId, long inc)
        {
            long id = (stamp << 23) + (workerId << 10) + inc;
            return id;
        }

        public static void Decode(long id, out long stamp, out long workerId, out long inc)
        {
            stamp = (id >> 23);
            id -= (stamp << 23);

            workerId = (id >> 10);
            id -= (workerId << 10);

            inc = id;
        }

        protected long NextId()
        {
            SnowflakeData d = this.snowflakeData;
            if (d.stamp == 0)
            {
                throw new Exception("d.stamp == 0");
            }

            long stamp = TimeUtils.GetTime();
            if (d.stamp < stamp)
            {
                d.stamp = stamp;
                d.inc = 0;
                return Encode(d.stamp, d.workerId, d.inc);
            }
            else if (d.stamp == stamp)
            {
                d.inc++;
                if (d.inc < MAX_INC)
                {
                    return Encode(d.stamp, d.workerId, d.inc);
                }
                else
                {
                    d.stamp++;
                    d.inc = 0;
                    return Encode(d.stamp, d.workerId, d.inc);
                }
            }
            else // d.stamp > stamp
            {
                if (d.inc < MAX_INC - 1)
                {
                    d.inc++;
                    return Encode(d.stamp, d.workerId, d.inc);
                }
                else
                {
                    d.stamp++;
                    d.inc = 0;
                    return Encode(d.stamp, d.workerId, d.inc);
                }
            }
        }
    }
}