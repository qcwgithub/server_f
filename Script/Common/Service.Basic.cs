using System;
using Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Driver.Core.Misc;

namespace Script
{
    // Service 提供给 IScript 数据、其他脚本的访问
    public abstract partial class Service
    {
        public bool IsDevelopment()
        {
            // return process.env.NODE_ENV == "development";
            return true;
        }

        public Task WaitAsync(int timeoutMs)
        {
            return Task.Delay(timeoutMs);
        }

        public async Task<ECode> WaitServiceConnectedAndStarted(ConnectToOtherService connectToOtherService, MsgType msgType)
        {
            int counter = 0;

            while (true)
            {
                if (this.data.state >= ServiceState.ShuttingDown)
                {
                    return ECode.ServiceIsShuttingDown;
                }

                counter++;
                if (counter == 2)
                {
                    this.logger.InfoFormat("{0} Wait connect to {1}...", msgType, connectToOtherService.to);
                }

                var r = await connectToOtherService.SendAsync(MsgType._GetServiceState, null);
                if (r.err != ECode.Success)
                {
                    await Task.Delay(100);
                    continue;
                }

                var res = r.CastRes<ResGetServiceState>();
                if (res.serviceState != ServiceState.Started)
                {
                    await Task.Delay(100);
                    continue;
                }

                if (counter >= 2)
                {
                    this.logger.InfoFormat("{0} Wait connect to {1}...Done", msgType, connectToOtherService.to);
                }
                break;
            }

            return ECode.Success;
        }

        public bool IsShuttingDown()
        {
            return this.data.state >= ServiceState.ShuttingDown;
        }

        public void SetState(ServiceState s)
        {
            this.data.state = s;
            this.logger.Info(s);
        }
    }
}