using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class Int64Range
    {
        [Key(0)]
        public long start;
        [Key(1)]
        public long end;

        public int Count()
        {
            return (int)(this.end - this.start + 1);
        }

        public bool Contains(long value)
        {
            return value >= this.start && value <= this.end;
        }

        public bool Contains(long value, out int index)
        {
            if (value >= this.start && value <= this.end)
            {
                index = (int)(value - this.start);
                return true;
            }

            index = -1;
            return false;
        }

        public long At(int index)
        {
            MyDebug.Assert(index >= 0 && index < this.Count());
            return this.start + index;
        }
    }

    [MessagePackObject]
    public class Int64RangeList
    {
        [Key(0)]
        public List<Int64Range> ranges;

        public int Count()
        {
            int count = 0;
            foreach (Int64Range range in this.ranges)
            {
                count += range.Count();
            }
            return count;
        }

        public void Add(long value)
        {
            if (this.ranges == null)
            {
                this.ranges = new List<Int64Range>();
            }

            if (this.ranges.Count == 0)
            {
                this.ranges.Add(new Int64Range { start = value, end = value });
                return;
            }

            for (int i = 0; i < this.ranges.Count; i++)
            {
                Int64Range range = this.ranges[i];
                if (range.Contains(value))
                {
                    break;
                }

                if (value < range.start)
                {
                    if (value == range.start - 1)
                    {
                        range.start--;
                    }
                    else
                    {
                        this.ranges.Insert(i, new Int64Range { start = value, end = value });
                    }
                    break;
                }
                else
                {
                    MyDebug.Assert(value > range.end);
                    if (value == range.end + 1)
                    {
                        if (i < this.ranges.Count - 1 && this.ranges[i + 1].start == range.end + 2)
                        {
                            var newRange = new Int64Range { start = range.start, end = this.ranges[i + 1].end };
                            this.ranges.RemoveAt(i);
                            this.ranges.RemoveAt(i);
                            this.ranges.Insert(i, newRange);
                        }
                        else
                        {
                            range.end++;
                        }
                        break;
                    }
                    else
                    {
                        if (i == this.ranges.Count - 1)
                        {
                            this.ranges.Add(new Int64Range { start = value, end = value });
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        public bool Contains(long value)
        {
            if (this.ranges == null)
            {
                return false;
            }

            foreach (Int64Range range in this.ranges)
            {
                if (range.Contains(value))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(long value, out int index)
        {
            index = -1;
            if (this.ranges == null)
            {
                return false;
            }

            int count = 0;
            foreach (Int64Range range in this.ranges)
            {
                if (range.Contains(value, out int range_index))
                {
                    index = range_index + count;
                    return true;
                }
                count += range.Count();
            }
            return false;
        }

        public long At(int index)
        {
            int count = 0;
            foreach (Int64Range range in this.ranges)
            {
                if (index >= count && index < count + range.Count())
                {
                    return range.At(index - count);
                }
                count++;
            }

            MyDebug.Assert(false);
            return 0;
        }

        /* Unit Test
        var random = new Random();
        for (int N = 1; N <= 10000; N++)
        {
            Console.WriteLine(N);
            List<long> list = new List<long>();
            for (int x = 1; x <= N; x++)
            {
                list.Add(N-x+1);
            }
            // list.Shuffle(random);

            for (int t = 0; t < 2; t++)
            {
                var gs = new Int64RangeList();
                foreach (int v in list)
                {
                    gs.Add(v);
                }

                if (gs.ranges.Count == 1 && gs.Count() == N)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("ERROR");
                }
            }
        }
        */
    }
}