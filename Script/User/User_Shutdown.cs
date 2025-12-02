using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_Shutdown : OnShutdown<UserService>
    {
        protected override async Task StopBusinesses()
        {
            this.service.logger.Info(nameof(StopBusinesses));

            UserServiceData usData = this.service.usData;

            //// allowNewUser = false
            usData.allowNewUser = false;

            await this.service.SendPSInfoToAAA(true, null);

            OnShutdown<Service>.s_ClearTimer(this.server, ref usData.timer_tick_loop);

            //// stop listening for client
            this.service.data.StopListenForClient_Tcp();
            this.service.data.StopListenForServer_Tcp();

            //// kick players
            List<long> kickList = new List<long>();
            this.service.logger.InfoFormat("start kick all userss, total {0}", usData.userDict.Count);
            List<Task<MyResponse>> tasks = new List<Task<MyResponse>>();
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

                foreach (long playerId in kickList)
                {
                    var msgD = MsgDestroyUser.Create(playerId, this.msgType.ToString(), new MsgKick { flags = LogoutFlags.CancelAutoLogin });
                    tasks.Add(this.service.connectToSelf.SendToSelfAsync(MsgType._PSDestroyPlayer, msgD));
                }

                await Task.WhenAll(tasks);

                kickList.Clear();
                tasks.Clear();

                await Task.Delay(100);

                this.service.logger.InfoFormat("left {0} users to kick", usData.userDict.Count);
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