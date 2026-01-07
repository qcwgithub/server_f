using Data;

namespace Script
{
    public partial class Service
    {
        // 每个服务需要实现此接口，把自己的业务结束掉
        protected virtual async Task StopBusinesses()
        {

        }

        protected void ClearTimer(ref Data.ITimer timer)
        {
            if (timer.IsAlive())
            {
                this.server.timerScript.ClearTimer(timer);
            }
            timer = null;
        }

        public static void s_ClearTimer(Server server, ref Data.ITimer timer)
        {
            if (timer.IsAlive())
            {
                server.timerScript.ClearTimer(timer);
            }
            timer = null;
        }

        void CheckTimer()
        {
            var serverData = this.server.data;

            // 等待定时器（本服务的）
            if (serverData.timerSData.timerDict.Count > 0)
            {
                foreach (var kv in serverData.timerSData.timerDict)
                {
                    TimerInfo timerInfo = kv.Value;
                    if (timerInfo.serviceId == this.serviceId)
                    {
                        this.logger.ErrorFormat("timer is still running! {0}", JsonUtils.stringify(timerInfo));
                    }
                }
            }
        }

        // 1 OnShutdown
        // 2 Timer
        // 3 Ctrl-C
        public async Task<ECode> Shutdown(bool force)
        {
            this.logger.Info($"Shutdown force {force}");

            if (this.data.state >= ServiceState.ShuttingDown)
            {
                this.logger.Info($"state is already >= {this.data.state}");
                return ECode.Success;
            }

            if (!force)
            {
                if (this.data.GetPassivelyConnections().Count > 0)
                {
                    this.data.markedShutdown = true;
                    this.logger.Info("set markedShutdown = true");
                    return ECode.Success;
                }
            }

            this.ClearTimer(ref this.data.timer_shutdown);

            //
            this.SetState(ServiceState.ShuttingDown);

            //----------------------------------------------
            // 关闭给别人提供的服务
            this.data.StopListenForServer_Tcp();
            // this.service.data.StopListenForServer_Ws();
            try
            {
                this.data.StopListen_Http();
            }
            catch (Exception ex)
            {
#if DEBUG
                if (Utils.IsWindows())
                {
                    // ignore
                }
                else
#endif
                {
                    throw ex;
                }
            }
            this.data.StopListenForClient_Tcp();
            try
            {
                this.data.StopListenForClient_Ws();
            }
            catch (Exception ex)
            {
#if DEBUG
                if (Utils.IsWindows())
                {
                    // ignore
                }
                else
#endif
                {
                    throw ex;
                }
            }
            //----------------------------------------------

            await this.StopBusinesses();

            this.ClearTimer(ref this.data.timer_CheckConnections);

            this.CheckTimer();

            // 等待 MessageDispatcher
            while (true)
            {
                bool busy = false;
                for (int i = 0; i < this.data.busyList.Count; i++)
                {
                    int v = this.data.busyList[i];
                    if (v != -1 &&
                        v != (int)MsgType._Service_Shutdown)
                    {
                        this.logger.InfoFormat("{0} is being handled, wait for it", (MsgType)v);
                        busy = true;
                    }
                }
                if (busy)
                {
                    await Task.Delay(500);
                }
                else
                {
                    break;
                }
            }

            this.CheckTimer();

            // 关闭所有主动发起的连接
            await this.data.CloseProactiveConnections();

            this.SetState(ServiceState.ReadyToShutdown);
            this.server.OnServiceSetStateToShutdown();
            return ECode.Success;
        }
    }
}