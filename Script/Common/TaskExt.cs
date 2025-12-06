using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Script
{
    public static class TaskExt
    {
        public static async void Forget(this Task task)
        {
            await task;
        }

        public static void Forget(this Task task, Service service)
        {
            service.dispatcher.Dispatch(null, MsgType._WaitTask, new MsgWaitTask { task = task }, null);
        }

        public static async Task Parallel(Func<int, Task> fillTask, Action<Task, Ptr<bool>> onTaskFinish = null)
        {
            var tasks = new List<Task>();
            int index = 0;
            var ptrBreak = new Ptr<bool>();
            while (true)
            {
                if (tasks.Count < 100)
                {
                    Task task = fillTask(index);
                    index++;
                    if (task == null)
                    {
                        break;
                    }
                    tasks.Add(task);
                }

                while (tasks.Count > 50)
                {
                    Task t = await Task.WhenAny(tasks);
                    tasks.Remove(t);

                    if (onTaskFinish != null)
                    {
                        onTaskFinish(t, ptrBreak);
                        if (ptrBreak.value)
                        {
                            break;
                        }
                    }
                }

                if (ptrBreak.value)
                {
                    break;
                }
            }

            await Task.WhenAll(tasks);
            if (onTaskFinish != null)
            {
                foreach (var t in tasks)
                {
                    onTaskFinish(t, ptrBreak);
                }
            }
        }

        public static async Task ParallelSmall(Func<int, Task> fillTask, Action<Task, Ptr<bool>> onTaskFinish = null)
        {
            var tasks = new List<Task>();
            int index = 0;
            var ptrBreak = new Ptr<bool>();
            while (true)
            {
                if (tasks.Count < 10)
                {
                    Task task = fillTask(index);
                    index++;
                    if (task == null)
                    {
                        break;
                    }
                    tasks.Add(task);
                }

                while (tasks.Count > 5)
                {
                    Task t = await Task.WhenAny(tasks);
                    tasks.Remove(t);

                    if (onTaskFinish != null)
                    {
                        onTaskFinish(t, ptrBreak);
                        if (ptrBreak.value)
                        {
                            break;
                        }
                    }
                }

                if (ptrBreak.value)
                {
                    break;
                }
            }

            await Task.WhenAll(tasks);
            if (onTaskFinish != null)
            {
                foreach (var t in tasks)
                {
                    onTaskFinish(t, ptrBreak);
                }
            }
        }
    }
}