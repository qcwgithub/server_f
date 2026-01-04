namespace Data
{
    public partial class TimerSData
    {
        void OnTick()
        {
            if (this.triggerDict.Count == 0)
            {
                return;
            }

            long nowS = TimeUtils.GetTimeS();
            if (nowS < this.minTimeS)
            {
                return;
            }

            List<TimerInfo> list = this.triggerDict[this.minTimeS];
            this.triggerDict.Remove(this.minTimeS);

            this.tempList.Clear();
            foreach (TimerInfo info in list)
            {
                this.tempList.Add((info.serviceId, info.timerType, info.data));
            }

            // reset minTimeS to a big value
            this.minTimeS = long.MaxValue;

            // update minTimeS
            foreach (var kv in this.triggerDict)
            {
                this.minTimeS = kv.Key;
                break;
            }

            foreach (TimerInfo info in list)
            {
                if (!info.loop)
                {
                    this.ClearTimer(info);
                }
                else
                {
                    info.nextTimeS = nowS + info.timeoutS;
                    this.AddTrigger(info);
                }
            }

            // trigger at last
            if (this.callback != null)
            {
                foreach (var tuple in this.tempList)
                {
                    this.callback(tuple.Item1, tuple.Item2, tuple.Item3);
                }
            }
            this.tempList.Clear();
        }

        private void AddTrigger(TimerInfo info)
        {
            MyDebug.Assert(info.timerId > 0);

            if (info.nextTimeS < this.minTimeS)
            {
                this.minTimeS = info.nextTimeS;
            }

            List<TimerInfo>? list;
            if (!this.triggerDict.TryGetValue(info.nextTimeS, out list))
            {
                list = new List<TimerInfo>();
                this.triggerDict.Add(info.nextTimeS, list);
            }
            list.Add(info);
        }

        public ITimer SetTimer(int serviceId, int timeoutS, TimerType timerType, object? data, bool loop)
        {
            if (timeoutS < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (timeoutS == 0 && loop)
            {
                throw new InvalidOperationException();
            }

            var info = new TimerInfo
            {
                timerId = this.nextId++,
                serviceId = serviceId,
                timeoutS = timeoutS,
                nextTimeS = TimeUtils.GetTimeS() + timeoutS,
                timerType = timerType,
                data = data,
                loop = loop
            };
            this.timerDict.Add(info.timerId, info);

            this.AddTrigger(info);

            return info;
        }

        public void ClearTimer(ITimer? timer)
        {
            MyDebug.Assert(timer.IsAlive());
            if (!timer.IsAlive())
            {
                return;
            }

            TimerInfo info = (TimerInfo)timer!;
            this.timerDict.Remove(info.timerId);
            List<TimerInfo>? list;
            if (this.triggerDict.TryGetValue(info.nextTimeS, out list))
            {
                int index = list.FindIndex(ele => ele.timerId == info.timerId);
                if (index >= 0)
                {
                    list.RemoveAt(index);
                }
            }

            // mark dead
            info.timerId = 0;
        }
    }
}