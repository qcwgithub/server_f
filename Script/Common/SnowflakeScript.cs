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
        public const int WORKER_ID_BITS = 10;
        public const int SEQ_BITS = 12;

        public const long MIN_STAMP = 1;
        public const long MAX_STAMP = (1L << STAMP_BITS) - 1;

        public const long MIN_WORKER_ID = 1; // 其实可以0，但是如果缺少配置也是0，干脆1
        public const long MAX_WORKER_ID = (1L << WORKER_ID_BITS) - 1;

        public const long MIN_SEQ = 1;
        public const long MAX_SEQ = (1L << SEQ_BITS) - 1;

        public static long Encode(long stamp, long workerId, long seq)
        {
            long id = (stamp << (WORKER_ID_BITS + SEQ_BITS)) + (workerId << SEQ_BITS) + seq;

#if DEBUG
            MyDebug.Assert(Decode(id, out long s, out long w, out long i));
            MyDebug.Assert(s == stamp);
            MyDebug.Assert(w == workerId);
            MyDebug.Assert(i == seq);
#endif

            return id;
        }

        public static bool Decode(long id, out long stamp, out long workerId, out long seq)
        {
            if (id < 0)
            {
                stamp = default;
                workerId = default;
                seq = default;
                return false;
            }

            stamp = (id >> (WORKER_ID_BITS + SEQ_BITS));
            id -= (stamp << (WORKER_ID_BITS + SEQ_BITS));

            workerId = (id >> SEQ_BITS);
            id -= (workerId << SEQ_BITS);

            seq = id;

            if (stamp < MIN_STAMP)
            {
                return false;
            }

            if (workerId < MIN_WORKER_ID)
            {
                return false;
            }

            if (seq < MIN_SEQ)
            {
                return false;
            }

            return true;
        }

        public static bool CheckValid(long id)
        {
            return Decode(id, out long stamp, out long workerId, out long seq);
        }

        protected bool InitSnowflakeData(long stamp, long workerId)
        {
            if (stamp < MIN_STAMP || stamp > MAX_STAMP)
            {
                this.service.logger.Error("stamp < MIN_STAMP || stamp > MAX_STAMP");
                return false;
            }

            if (workerId < MIN_WORKER_ID || workerId > MAX_WORKER_ID)
            {
                this.service.logger.Error("workerId < MIN_WORKER_ID || workerId > MAX_WORKER_ID");
                return false;
            }

            this.snowflakeData.stamp = stamp;
            this.snowflakeData.workerId = workerId;
            this.snowflakeData.seq = MIN_SEQ;
            return true;
        }

        long baseStamp = 0;
        protected long NowSnowflakeStamp()
        {
            if (this.baseStamp == 0)
            {
                DateTime baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                this.baseStamp = TimeUtils.DateTimeToMilliseconds(baseDate);
            }

            long now = TimeUtils.GetTime();
            return now - this.baseStamp;
        }

        protected long NextId()
        {
            SnowflakeData d = this.snowflakeData;
            if (d.stamp < MIN_STAMP)
            {
                throw new Exception("d.stamp < MIN_STAMP");
            }
            if (d.workerId < MIN_WORKER_ID)
            {
                throw new Exception("d.workerId < MIN_WORKER_ID");
            }
            if (d.seq < MIN_SEQ)
            {
                throw new Exception("d.seq < MIN_SEQ");
            }

            long stamp = this.NowSnowflakeStamp();
            if (d.stamp < stamp)
            {
                d.stamp = stamp;
                d.seq = MIN_SEQ;
                return Encode(d.stamp, d.workerId, d.seq);
            }
            else if (d.stamp == stamp)
            {
                d.seq++;
                if (d.seq < MAX_SEQ)
                {
                    return Encode(d.stamp, d.workerId, d.seq);
                }
                else
                {
                    d.stamp++;
                    d.seq = MIN_SEQ;
                    return Encode(d.stamp, d.workerId, d.seq);
                }
            }
            else // d.stamp > stamp
            {
                if (d.seq < MAX_SEQ - 1)
                {
                    d.seq++;
                    return Encode(d.stamp, d.workerId, d.seq);
                }
                else
                {
                    d.stamp++;
                    d.seq = MIN_SEQ;
                    return Encode(d.stamp, d.workerId, d.seq);
                }
            }
        }
    }
}