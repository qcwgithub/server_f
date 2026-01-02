using Data;

namespace Script
{
    public partial class UserService
    {
        protected override async Task StopBusinesses()
        {
            this.logger.Info("StopBusinesses");

            //// allowNewUser = false
            this.sd.allowNewUser = false;

            s_ClearTimer(this.server, ref this.sd.timer_tick_loop);

            //// stop listening for client
            this.data.StopListenForClient_Tcp();
            this.data.StopListenForServer_Tcp();

            //// kick players
            List<User> kickList = new();
            this.logger.InfoFormat("start kick all userss, total {0}", this.sd.userCount);
            List<Task> tasks = new List<Task>();
            while (true)
            {
                foreach (var kv in this.sd.userDict)
                {
                    if (this.sd.IsUserLocked(kv.Key))
                    {
                        continue;
                    }

                    User user = kv.Value;
                    kickList.Add(user);
                    if (kickList.Count >= 10)
                    {
                        break;
                    }
                }

                if (kickList.Count > 0)
                {
                    foreach (User user in kickList)
                    {
                        tasks.Add(this.DestroyUser(user, UserDestroyUserReason.Shutdown));
                    }

                    await Task.WhenAll(tasks);

                    kickList.Clear();
                    tasks.Clear();
                }
                await Task.Delay(100);
                this.logger.InfoFormat("left {0} users to kick", this.sd.userCount);

                if (this.sd.userCount == 0)
                {
                    break;
                }
            }

            ////
            while (true)
            {
                int pendingCount = this.server.data.ioThread.PendingCount();
                this.logger.InfoFormat("IO Thread pending {0} finished {1} total {2}", pendingCount,
                    this.server.data.ioThread.FinishedCount(),
                    this.server.data.ioThread.TotalCount());
                if (pendingCount == 0)
                {
                    break;
                }

                await Task.Delay(1000);
            }
        }
    }
}