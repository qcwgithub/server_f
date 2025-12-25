using Data;

namespace Script
{
    public class Room_Shutdown : OnShutdown<RoomService>
    {
        public Room_Shutdown(Server server, RoomService service) : base(server, service)
        {
        }

        protected override async Task StopBusinesses()
        {
            this.service.logger.Info("StopBusinesses");

            RoomServiceData usData = this.service.sd;

            //// allowNewRoom = false
            usData.allowNewRoom = false;

            OnShutdown<Service>.s_ClearTimer(this.server, ref usData.timer_tick_loop);

            //// stop listening for client
            this.service.data.StopListenForClient_Tcp();
            this.service.data.StopListenForServer_Tcp();

            //// kick players
            List<long> kickList = new List<long>();
            this.service.logger.InfoFormat("start kick all roomss, total {0}", usData.roomCount);
            List<Task> tasks = new List<Task>();
            while (true)
            {
                foreach (var kv in usData.roomDict)
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

                foreach (long roomId in kickList)
                {
                    var msgD = new MsgRoomDestroyRoom();
                    msgD.roomId = roomId;
                    msgD.reason = RoomDestroyRoomReason.Shutdown;
                    tasks.Add(this.service.connectToSelf.Request<MsgRoomDestroyRoom, ResRoomDestroyRoom>(MsgType._Room_DestroyRoom, msgD));
                }

                await Task.WhenAll(tasks);

                kickList.Clear();
                tasks.Clear();

                await Task.Delay(100);

                this.service.logger.InfoFormat("left {0} rooms to kick", usData.roomCount);
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