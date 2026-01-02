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
            List<long> kickList = new List<long>();
            this.logger.InfoFormat("start kick all userss, total {0}", this.sd.userCount);
            List<Task> tasks = new List<Task>();
            while (true)
            {
                foreach (var kv in this.sd.userDict)
                {
                    kickList.Add(kv.Key);
                    if (kickList.Count >= 10)
                    {
                        break;
                    }
                }

                if (kickList.Count == 0)
                {
                    break;
                }

                foreach (long userId in kickList)
                {
                    tasks.Add(this.DestroyUser(userId, UserDestroyUserReason.Shutdown));
                }

                await Task.WhenAll(tasks);

                kickList.Clear();
                tasks.Clear();

                await Task.Delay(100);

                this.logger.InfoFormat("left {0} users to kick", this.sd.userCount);
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
                else
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}