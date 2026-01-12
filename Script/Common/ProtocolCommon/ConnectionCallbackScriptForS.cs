using Data;

namespace Script
{
    public class ConnectionCallbackScriptForS : ConnectionCallbackScript
    {
        public ConnectionCallbackScriptForS(Server server, Service service) : base(server, service)
        {
        }

        public bool IsServiceConnected(int serviceId)
        {
            if (!this.service.data.otherServiceConnections.TryGetValue(serviceId, out var connection) || !connection.IsConnected())
            {
                return false;
            }
            return true;
        }

        public IServiceConnection? RandomOtherServiceConnection(ServiceType serviceType)
        {
            List<IServiceConnection> list = this.service.data.otherServiceConnections2[(int)serviceType];
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