using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class IntRange
    {
        [Key(0)]
        public int start;
        [Key(1)]
        public int end;

        public int Count()
        {
            return (int)(this.end - this.start + 1);
        }

        public bool Contains(int value)
        {
            return value >= this.start && value <= this.end;
        }
        public bool Contains(int value, out int index)
        {
            if (value >= this.start && value <= this.end)
            {
                index = (int)(value - this.start);
                return true;
            }

            index = -1;
            return false;
        }

        public int At(int index)
        {
            MyDebug.Assert(index >= 0 && index < this.Count());
            return this.start + index;
        }
    }

    [MessagePackObject]
    public class IntRangeList
    {
        [Key(0)]
        public List<IntRange> ranges;

        public long Count()
        {
            long count = 0;
            foreach (IntRange range in this.ranges)
            {
                count += range.end - range.start + 1;
            }
            return count;
        }

        public void Add(int value)
        {
            if (this.ranges == null)
            {
                this.ranges = new List<IntRange>();
            }

            if (this.ranges.Count == 0)
            {
                this.ranges.Add(new IntRange { start = value, end = value });
                return;
            }

            for (int i = 0; i < this.ranges.Count; i++)
            {
                IntRange range = this.ranges[i];
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
                        this.ranges.Insert(i, new IntRange{ start = value, end = value });
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
                            var newRange = new IntRange { start = range.start, end = this.ranges[i + 1].end };
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
                            this.ranges.Add(new IntRange{ start = value, end = value });
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

        public bool Contains(int value)
        {
            if (this.ranges == null)
            {
                return false;
            }

            foreach (IntRange range in this.ranges)
            {
                if (range.Contains(value))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(int value, out int index)
        {
            index = -1;
            if (this.ranges == null)
            {
                return false;
            }

            int count = 0;
            foreach (IntRange range in this.ranges)
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

        public int At(int index)
        {
            int count = 0;
            foreach (IntRange range in this.ranges)
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
    }
}