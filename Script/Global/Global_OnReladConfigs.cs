using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Global_OnReladConfigs : OnReloadConfigs<GlobalService>
    {
        public override async Task<MyResponse> Handle(ProtocolClientData socket, MsgReloadConfigs msg)
        {
            MyResponse r = await base.Handle(socket, msg);
            if (r.err != ECode.Success)
            {
                return r;
            }

            return r;
        }

        public async Task Shared_OnReload_NormalServerStatusConfigs()
        {
            var broadcast = new A_ResGetServiceConfigs();
            broadcast.res = await this.service.CreateResGetServiceConfigs();

            // 自己也走相同逻辑
            this.service.data.SaveServiceConfigs(broadcast.res);
            await this.BroadcastToAll(MsgType._A_ResGetServiceConfigs, broadcast);
        }

        async Task BroadcastToAll(MsgType msgType, object msg)
        {
            // GroupServiceData sd = this.service.groupServiceData;

            // var tasks = new List<Task>();

            // foreach (List<ProtocolClientData> list in sd.otherServiceSockets2)
            // {
            //     if (list == null)
            //     {
            //         continue;
            //     }

            //     foreach (ProtocolClientData soc in list)
            //     {
            //         if (soc == null || soc.serviceTypeAndId == null || !soc.IsConnected())
            //         {
            //             continue;
            //         }

            //         tasks.Add(soc.SendAsync(msgType, msg, null));
            //     }
            // }

            // foreach (var kv in sd.normalSockets)
            // {
            //     foreach (ProtocolClientData soc in kv.Value)
            //     {
            //         if (soc == null || soc.serviceTypeAndId == null || !soc.IsConnected())
            //         {
            //             continue;
            //         }

            //         tasks.Add(soc.SendAsync(msgType, msg, null));
            //     }
            // }

            // await Task.WhenAll(tasks);
        }
    }
}