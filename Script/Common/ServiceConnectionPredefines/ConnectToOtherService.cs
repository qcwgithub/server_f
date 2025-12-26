using Data;

namespace Script
{
    public abstract class ConnectToOtherService
    {
        public readonly Service self;
        public readonly ServiceType to;
        public ConnectToOtherService(Service self, ServiceType to)
        {
            this.self = self;
            this.to = to;

            // 重点
            // 要发送给他，必须有定义连接他才行
            MyDebug.Assert(self.data.connectToServiceTypes.Contains(to));
        }
    }
}