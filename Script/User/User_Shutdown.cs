using Data;

namespace Script
{
    public class User_Shutdown : OnShutdown<UserService>
    {
        public User_Shutdown(Server server, UserService service) : base(server, service)
        {
        }

        protected override async Task StopBusinesses()
        {
            this.service.logger.Info("StopBusinesses");

            UserServiceData usData = this.service.sd;

            //// allowNewUser = false
            usData.allowNewUser = false;

            OnShutdown<Service>.s_ClearTimer(this.server, ref usData.timer_tick_loop);

            //// stop listening for client
            this.service.data.StopListenForClient_Tcp();
            this.service.data.StopListenForServer_Tcp();

            //// kick players
            List<long> kickList = new List<long>();
            this.service.logger.InfoFormat("start kick all userss, total {0}", usData.userCount);
            List<Task> tasks = new List<Task>();
            while (true)
            {
                foreach (var kv in usData.userDict)
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
                    var msgD = new MsgUserDestroyUser();
                    msgD.userId = userId;
                    msgD.reason = UserDestroyUserReason.Shutdown;
                    tasks.Add(this.service.connectToSelf.Request<MsgUserDestroyUser, ResUserDestroyUser>(MsgType._User_DestroyUser, msgD));
                }

                await Task.WhenAll(tasks);

                kickList.Clear();
                tasks.Clear();

                await Task.Delay(100);

                this.service.logger.InfoFormat("left {0} users to kick", usData.userCount);
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