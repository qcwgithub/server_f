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
            service.WaitTask(task);
        }
    }
}