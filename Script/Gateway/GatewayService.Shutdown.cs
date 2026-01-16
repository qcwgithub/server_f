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
            while (true)
            {
                foreach (var kv in this.sd.userDict)
                {
                    GatewayUser user = kv.Value;
                    kickList.Add(user);
                }

                if (kickList.Count > 0)
                {
                    foreach (GatewayUser user in kickList)
                    {
                        this.DestroyUser(user, GatewayDestroyUserReason.Shutdown, new MsgKick { flags = LogoutFlags.None });
                    }

                    kickList.Clear();
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