using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using System;

namespace Script
{
    public abstract class OnShutdown<S> : Handler<S, MsgShutdown>
        where S : Service
    {
        public override MsgType msgType => MsgType._Shutdown;

        // 每个服务需要实现此接口，把自己的业务结束掉
        protected abstract Task StopBusinesses();

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
                    if (timerInfo.serviceId == this.service.serviceId)
                    {
                        this.service.logger.ErrorFormat("timer is still running! {0}", JsonUtils.stringify(timerInfo));
                    }
                }
            }
        }

        public sealed override async Task<MyResponse> Handle(ProtocolClientData socket, MsgShutdown msg)
        {
            this.service.logger.Info($"{this.msgType} force {msg.force}");

            if (this.service.data.state >= ServiceState.ShuttingDown)
            {
                this.service.logger.Info($"state is already >= {this.service.data.state}");
                return ECode.Success;
            }

            if (!msg.force)
            {
                if (this.service.data.GetPassivelyConnections().Count > 0)
                {
                    this.service.data.markedShutdown = true;
                    this.service.logger.Info("set markedShutdown = true");
                    return ECode.Success;
                }
            }

            this.ClearTimer(ref this.service.data.timer_shutdown);

            //
            this.service.SetState(ServiceState.ShuttingDown);

            //----------------------------------------------
            // 关闭给别人提供的服务
            this.service.data.StopListenForServer_Tcp();
            // this.service.data.StopListenForServer_Ws();
            try
            {
                this.service.data.StopListen_Http();
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
            this.service.data.StopListenForClient_Tcp();
            try
            {
                this.service.data.StopListenForClient_Ws();
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

            this.ClearTimer(ref this.service.data.timer_CheckConnections_Loop);

            this.CheckTimer();

            // 等待 MessageDispatcher
            while (true)
            {
                bool busy = false;
                for (int i = 0; i < this.service.data.busyList.Count; i++)
                {
                    int v = this.service.data.busyList[i];
                    if (v != -1 &&
                        v != (int)MsgType._Shutdown &&
                        v != (int)MsgType._Start)
                    {
                        this.service.logger.InfoFormat("{0} is being handled, wait for it", (MsgType)v);
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
            await this.service.data.CloseProactiveConnections();

            this.service.SetState(ServiceState.ReadyToShutdown);
            this.server.feiShuMessenger.SendEventMessage(this.service.data.serviceTypeAndId.ToString() + " ReadyToShutdown");
            this.server.OnServiceSetStateToShutdown();
            return ECode.Success;
        }
    }
}