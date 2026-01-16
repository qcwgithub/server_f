using Data;

namespace Script
{
    public partial class GatewayService
    {
        protected override async Task StopBusinesses()
        {
            this.logger.Info("StopBusinesses");

            //// stop listening for client
            this.sd.StopListenForClient_Tcp();

            //// kick players
            List<GatewayUser> kickList = new();
            this.logger.InfoFormat("start kick all userss, total {0}", this.sd.userCount);
            List<Task> tasks = new List<Task>();
            while (true)
            {
                foreach (var kv in this.sd.userDict)
                {
                    // if (this.sd.IsUserLocked(kv.Key))
                    // {
                    //     continue;
                    // }

                    GatewayUser user = kv.Value;
                    kickList.Add(user);
                    if (kickList.Count >= 10)
                    {
                        break;
                    }
                }

                if (kickList.Count > 0)
                {
                    foreach (GatewayUser user in kickList)
                    {
                        tasks.Add(this.DestroyUser(user.userId, GatewayDestroyUserReason.Shutdown, new MsgKick { flags = LogoutFlags.None }));
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
        }
    }
}