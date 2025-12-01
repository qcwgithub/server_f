using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class NormalProtocolClientScriptS : ProtocolClientScriptS
    {
        public ProtocolClientData RandomGroupSocket(ServiceType serviceType)
        {
            MyDebug.Assert(serviceType.IsGroupService());

            NormalServiceData serviceData = (NormalServiceData)this.service.data;
            List<ProtocolClientData> sockets = serviceData.GetGroupSockets(serviceType);
            if (sockets == null || sockets.Count == 0)
            {
                return null;
            }

            int index = SCUtils.WeightedRandomSimple(this.service.data.random, sockets.Count, i =>
            {
                if (sockets[i].IsConnected())
                {
                    return 1;
                }
                return 0;
            });

            if (index == -1)
            {
                return null;
            }
            return sockets[index];
        }

        public async Task<MyResponse> SendToGroupServiceAsync(ServiceType serviceType, MsgType type, object msg)
        {
            MyDebug.Assert(serviceType.IsGroupService());

            ProtocolClientData socket = this.RandomGroupSocket(serviceType);
            if (socket == null)
            {
                return ECode.Server_NotConnected;
            }
            MyResponse r = await socket.SendAsync(type, msg, pTimeoutS: null);
            if (r.err == ECode.Server_Timeout)
            {
                this.service.logger.ErrorFormat("send {0} to {1} Timeout", type.ToString(), socket.serviceTypeAndId.Value.ToString());
            }

            return r;
        }
    }
}