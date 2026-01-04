using Data;

namespace Script
{
    public partial class Service
    {
        void StartKeepConnections()
        {
            MyDebug.Assert(!this.data.timer_CheckConnections.IsAlive());
            this.data.timer_CheckConnections = this.server.timerScript.SetTimer(this.serviceId, 0, TimerType.CheckConnections, null);
        }

        public virtual async Task<ECode> Start()
        {
            try
            {
                this.SetState(ServiceState.Starting);

                long startS = TimeUtils.GetTimeS();

                ECode e = ECode.Success;

                e = await this.InitServiceConfigsUntilSuccess();
                if (e == ECode.Success)
                {
                    // this.service.logger.InfoFormat("{0} StartKeepConnections", this.msgType);
                    this.StartKeepConnections();

                    foreach (var serviceType in this.data.connectToServiceTypes)
                    {
                        if (!this.serviceProxyDict.TryGetValue(serviceType, out ServiceProxy? serviceProxy) || serviceProxy == null)
                        {
                            MyDebug.Assert(false);
                            continue;
                        }

                        // 代码中写着要连接，但是服务器配置里却没有启动这个服务，此时不需要等
                        if (!this.data.current_resGetServiceConfigs.ExistServiceType(serviceType))
                        {
                            continue;
                        }

                        e = await this.WaitServiceConnectedAndStarted(serviceProxy);
                        if (e != ECode.Success)
                        {
                            break;
                        }
                    }
                }

                long start2S = TimeUtils.GetTimeS();

                if (e == ECode.Success)
                {
                    e = await this.Start2();
                }

                if (e == ECode.Success)
                {
                    // this.service.logger.InfoFormat("{0} ListenForServer_Tcp", this.msgType);
                    this.data.ListenForServer_Tcp();
                }

                if (e != ECode.Success)
                {
                    // 如果已经在关闭中，交给 OnShutdown 处理
                    if (e != ECode.ServiceIsShuttingDown)
                    {
                        await this.OnErrorExit(e);
                    }
                    else
                    {
                        this.logger.InfoFormat("ECode.{0}", e);
                    }
                    return e;
                }

                long endS = TimeUtils.GetTimeS();
                this.logger.InfoFormat("waitS({0}s) selfS({1}s)", start2S - startS, endS - start2S);

                this.SetState(ServiceState.Started);
                this.server.feiShuMessenger.SendEventMessage(this.data.serviceTypeAndId.ToString() + " Started");

                return ECode.Success;
            }
            catch (Exception ex)
            {
                await this.OnErrorExit(ex);
                return ECode.Exception;
            }
        }

        protected virtual Task<ECode> Start2()
        {
            return Task.FromResult(ECode.Success);
        }
    }
}