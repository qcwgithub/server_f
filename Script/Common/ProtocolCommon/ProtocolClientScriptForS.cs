using Data;

namespace Script
{
    public class ProtocolClientScriptForS : ProtocolClientScript
    {
        public ProtocolClientScriptForS(Server server, Service service) : base(server, service)
        {
        }

        public ProtocolClientData CreateConnector(IProtocolClientCallbackProvider callbackProvider, string ip, int port)
        {
            var socket = new TcpClientData();
            socket.ConnectorInit(callbackProvider, ip, port);
            return socket;
        }

        public bool IsServiceConnected(int serviceId)
        {
            if (!this.service.data.otherServiceConnections.TryGetValue(serviceId, out var connection) || !connection.IsConnected())
            {
                return false;
            }
            return true;
        }

        public ServiceConnection? RandomOtherServiceConnection(ServiceType serviceType)
        {
            List<ServiceConnection> list = this.service.data.otherServiceConnections2[(int)serviceType];
            if (list == null || list.Count == 0)
            {
                return null;
            }

            int index = SCUtils.WeightedRandomSimple(this.service.data.random, list.Count, i =>
            {
                if (list[i].IsConnected())
                {
                    return 1;
                }
                return 0;
            });

            if (index == -1)
            {
                return null;
            }
            return list[index];
        }
    }
}