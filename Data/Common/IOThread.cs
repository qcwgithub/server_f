using System;
using System.Collections.Generic;
using System.Threading;
namespace Data
{
    public class IOThread
    {
        Thread thread;
        EventWaitHandle waitHandle;
        List<Action> actions;
        int busy;
        List<Exception> exceptions;

        int total;
        int finished;

        public IOThread()
        {
            this.waitHandle = new AutoResetEvent(false);
            this.exceptions = new List<Exception>();
        }

        void Start()
        {
            this.thread = new Thread(this.ThreadMain);
            this.actions = new List<Action>();
            this.busy = 0;
            this.thread.Start();
        }

        void ThreadMain()
        {
            try
            {
                List<Action> runList = new List<Action>();
                while (true)
                {
                    this.waitHandle.WaitOne();

                    lock (this.actions)
                    {
                        runList.AddRange(this.actions);
                        this.actions.Clear();
                    }

                    this.busy = runList.Count;
                    this.total += runList.Count;

                    foreach (Action action in runList)
                    {
                        try
                        {
                            action();
                            this.finished++;
                        }
                        catch (Exception ex)
                        {
                            lock (this.exceptions)
                            {
                                this.exceptions.Add(ex);
                            }
                        }

                        this.busy--;
                    }

                    runList.Clear();
                    this.busy = 0;
                }
            }
            catch (Exception ex)
            {
                this.busy = 0;
                lock (this.exceptions)
                {
                    this.exceptions.Add(ex);
                }
            }
        }

        public void Run(Action action)
        {
            if (this.thread != null && !this.thread.IsAlive)
            {
                this.thread = null;
            }

            if (this.thread == null)
            {
                this.Start();
            }

            lock (this.actions)
            {
                this.actions.Add(action);
            }
            this.waitHandle.Set();
        }

        public int PendingCount()
        {
            if (this.thread == null)
            {
                return 0;
            }

            lock (this.actions)
            {
                return this.actions.Count + this.busy;
            }
        }

        public int FinishedCount()
        {
            return this.finished;
        }

        public int TotalCount()
        {
            return this.total;
        }

        public void GetExceptions(List<Exception> list)
        {
            lock (this.exceptions)
            {
                list.AddRange(this.exceptions);
                this.exceptions.Clear();
            }
        }
    }
}