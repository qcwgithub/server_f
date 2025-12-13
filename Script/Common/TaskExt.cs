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
            service.dispatcher.Dispatch<MsgWaitTask, ResWaitTask>(service.data.localConnection, MsgType._WaitTask, new MsgWaitTask { task = task });
        }
    }
}