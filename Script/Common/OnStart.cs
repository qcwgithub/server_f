using System;
using System.Threading.Tasks;
using Data;
using System.Collections.Generic;
using System.Linq;

namespace Script
{
    public abstract class OnStart<S> : Handler<S, MsgStart, ResStart>
        where S : Service
    {
        protected OnStart(Server server, S service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Service_Start;

        void StartKeepConnections()
        {
            MyDebug.Assert(!this.service.data.timer_CheckConnections_Loop.IsAlive());
            this.service.data.timer_CheckConnections_Loop = this.server.timerScript.SetTimer(this.service.serviceId, 0, MsgType._Service_CheckConnections_Loop, null);
        }

        public override async Task<ECode> Handle(MessageContext context, MsgStart msg, ResStart res)
        {
            try
            {
                this.service.SetState(ServiceState.Starting);

                long startS = TimeUtils.GetTimeS();

                ECode e = ECode.Success;

                e = await this.service.InitServiceConfigsUntilSuccess();
                if (e == ECode.Success)
                {
                    // this.service.logger.InfoFormat("{0} StartKeepConnections", this.msgType);
                    this.StartKeepConnections();

                    foreach (var serviceType in this.service.data.connectToServiceTypes)
                    {
                        if (!this.service.serviceProxyDict.TryGetValue(serviceType, out ServiceProxy? serviceProxy) || serviceProxy == null)
                        {
                            MyDebug.Assert(false);
                            continue;
                        }

                        // 代码中写着要连接，但是服务器配置里却没有启动这个服务，此时不需要等
                        if (!this.service.data.current_resGetServiceConfigs.ExistServiceType(serviceType))
                        {
                            continue;
                        }

                        e = await this.service.WaitServiceConnectedAndStarted(serviceProxy, this.msgType);
                        if (e != ECode.Success)
                        {
                            break;
                        }
                    }
                }

                long start2S = TimeUtils.GetTimeS();

                if (e == ECode.Success)
                {
                    e = await this.Handle2();
                }

                if (e == ECode.Success)
                {
                    // this.service.logger.InfoFormat("{0} ListenForServer_Tcp", this.msgType);
                    this.service.data.ListenForServer_Tcp();
                }

                if (e != ECode.Success)
                {
                    // 如果已经在关闭中，交给 OnShutdown 处理
                    if (e != ECode.ServiceIsShuttingDown)
                    {
                        await base.OnErrorExit(e);
                    }
                    else
                    {
                        this.service.logger.InfoFormat("{0} ECode.{1}", this.msgType, e);
                    }
                    return e;
                }

                long endS = TimeUtils.GetTimeS();
                this.service.logger.InfoFormat("{0} waitS({1}s) selfS({2}s)", this.msgType, start2S - startS, endS - start2S);

                this.service.SetState(ServiceState.Started);
                this.server.feiShuMessenger.SendEventMessage(this.service.data.serviceTypeAndId.ToString() + " Started");

                return ECode.Success;
            }
            catch (Exception ex)
            {
                await base.OnErrorExit(ex);
                return ECode.Exception;
            }
        }

        protected virtual Task<ECode> Handle2()
        {
            return Task.FromResult(ECode.Success);
        }
    }
}