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

        public const int STAMP_BITS = 41;
        public const int WORKER_ID_BITS = 13;
        public const int INC_BITS = 10;

        public static readonly long MAX_STAMP = (1 << STAMP_BITS) - 1;
        public static readonly long MAX_WORKER_ID = (1 << WORKER_ID_BITS) - 1;
        public static readonly long MAX_INC = (1 << INC_BITS) - 1;

        // 1 41 13 10
        public static long Encode(long stamp, long workerId, long inc)
        {
            long id = (stamp << (WORKER_ID_BITS + INC_BITS)) + (workerId << INC_BITS) + inc;

#if DEBUG
            MyDebug.Assert(Decode(id, out long s, out long w, out long i));
            MyDebug.Assert(s == stamp);
            MyDebug.Assert(w == workerId);
            MyDebug.Assert(i == inc);
#endif

            return id;
        }

        public static bool Decode(long id, out long stamp, out long workerId, out long inc)
        {
            if (id < 0)
            {
                stamp = 0;
                workerId = 0;
                inc = 0;
                return false;
            }

            stamp = (id >> (WORKER_ID_BITS + INC_BITS));
            id -= (stamp << (WORKER_ID_BITS + INC_BITS));

            workerId = (id >> INC_BITS);
            id -= (workerId << INC_BITS);

            inc = id;

            if (stamp <= 0)
            {
                return false;
            }

            if (workerId < 0)
            {
                return false;
            }

            if (inc < 0)
            {
                return false;
            }

            return true;
        }

        protected void InitSnowflakeData(long stamp, long workerId)
        {
            if (stamp <= 0 || stamp > MAX_STAMP)
            {
                throw new Exception("stamp <= 0 || stamp > MAX_STAMP");
            }

            if (workerId <= 0 || workerId > MAX_WORKER_ID)
            {
                throw new Exception("workerId <= 0 || workerId > MAX_WORKER_ID");
            }

            this.snowflakeData.stamp = stamp;
            this.snowflakeData.workerId = workerId;
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