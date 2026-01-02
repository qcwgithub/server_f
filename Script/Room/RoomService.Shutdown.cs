using Data;

namespace Script
{
    public partial class RoomService
    {
        protected override async Task StopBusinesses()
        {
            this.logger.Info("StopBusinesses");

            //// allowNewRoom = false
            this.sd.allowNewRoom = false;

            s_ClearTimer(this.server, ref this.sd.timer_tick_loop);

            //// stop listening for client
            this.data.StopListenForClient_Tcp();
            this.data.StopListenForServer_Tcp();

            //// kick players
            List<Room> kickList = new();
            this.logger.InfoFormat("start kick all roomss, total {0}", this.sd.roomCount);
            List<Task> tasks = new List<Task>();
            while (true)
            {
                foreach (var kv in this.sd.roomDict)
                {
                    if (this.sd.IsRoomLocked(kv.Key))
                    {
                        continue;
                    }
                    Room room = kv.Value;
                    kickList.Add(room);
                    if (kickList.Count >= 10)
                    {
                        break;
                    }
                }

                if (kickList.Count > 0)
                {
                    foreach (Room room in kickList)
                    {
                        tasks.Add(this.DestroyRoom(room, RoomDestroyRoomReason.Shutdown));
                    }

                    await Task.WhenAll(tasks);

                    kickList.Clear();
                    tasks.Clear();
                }

                await Task.Delay(100);

                this.logger.InfoFormat("left {0} rooms to kick", this.sd.roomCount);

                if (this.sd.roomCount == 0)
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
                else
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}