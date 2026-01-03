namespace Data
{
    public class ProtocolClientCallbackProviderRobotClient : IProtocolClientCallbackProvider
    {
        public IProtocolClientCallback? protocolClientCallbackRobotClient;

        public IProtocolClientCallback? GetProtocolClientCallback(ProtocolClientData protocolClientData)
        {
            return this.protocolClientCallbackRobotClient;
        }
    }
}