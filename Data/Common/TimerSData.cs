using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

namespace Data
{
    public interface ITimer
    {
    	
    }
    public static class ITimerEx
    {
        public static bool IsAlive(this ITimer? timer)
        {
            return timer != null && ((TimerInfo)timer).timerId != 0;
        }
    }

    public class TimerInfo : ITimer
    {
        public long timerId;
        public int serviceId;
        public int timeoutS;
        public long nextTimeS;
        public TimerType timerType;
        public object? data;
        public bool loop;
    }

    public class TimerDataComparer : IComparer<long>
    {
        public int Compare(long s1, long s2)
        {
            return (int)(s1 - s2);
        }
    }

    public partial class TimerSData
    {
        Action<int, TimerType, object> callback;
        public void SetTimerCallback(Action<int, TimerType, object> callback)
        {
            this.callback = callback;
        }

        public long nextId = 1;

        public readonly List<(int, TimerType, object)> tempList = new();
        public readonly SortedDictionary<long, List<TimerInfo>> triggerDict = new SortedDictionary<long, List<TimerInfo>>(new TimerDataComparer());
        public readonly Dictionary<long, TimerInfo> timerDict = new();
        
        public long minTimeS = long.MaxValue;

        public bool started { get; private set; }
        public int tickIntervalMs = 1000;
        public async void Start()
        {
            if (started) return;
            started = true;

            while (true)
            {
                await Task.Delay(tickIntervalMs);
                this.OnTick();
            }
        }
    }
}