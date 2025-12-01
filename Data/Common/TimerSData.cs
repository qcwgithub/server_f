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
        public static bool IsAlive(this ITimer timer)
        {
            return timer != null && ((TimerInfo)timer).timerId != 0;
        }
    }

    public class TimerInfo : ITimer
    {
        public long timerId;
        public int serviceId;
        public int timeoutS;
        public int nextTimeS;
        public MsgType msgType;
        public object msg;
        public bool loop;
    }

    public class TimerDataComparer : IComparer<int>
    {
        public int Compare(int s1, int s2)
        {
            return s1 - s2;
        }
    }

    public partial class TimerSData
    {
        Action<int, MsgType, object> callback;
        public void SetTimerCallback(Action<int, MsgType, object> callback)
        {
            this.callback = callback;
        }

        public long nextId = 1;

        public List<(int, MsgType, object)> tempList = new List<(int, MsgType, object)>();
        public SortedDictionary<int, List<TimerInfo>> triggerDict = new SortedDictionary<int, List<TimerInfo>>(new TimerDataComparer());
        public Dictionary<long, TimerInfo> timerDict = new Dictionary<long, TimerInfo>();
        
        public int minTimeS = int.MaxValue;

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