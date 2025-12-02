using System;
using Data;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Linq;

namespace Script
{
    public class GroupProtocolClientScriptS : ProtocolClientScriptS
    {
        #region send

        public ProtocolClientData RandomSocket(ServiceType serviceType)
        {
            MyDebug.Assert(serviceType.IsNormalService());

            GroupServiceData groupServiceData = (GroupServiceData)this.service.data;
            List<ProtocolClientData> sockets = groupServiceData.GetNormalSockets(serviceType);
            if (sockets == null || sockets.Count == 0)
            {
                return null;
            }

            var candidates = new List<ProtocolClientData>();
            foreach (var socket in sockets)
            {
                if (socket.IsConnected())
                {
                    candidates.Add(socket);
                }
            }
            if (candidates.Count == 0)
            {
                return null;
            }

            var selectedSocket = candidates[this.service.data.random.Next(0, candidates.Count)];
            return selectedSocket;
        }

        #endregion
    }
}