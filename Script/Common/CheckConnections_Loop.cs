using Data;
using System.Threading.Tasks;

namespace Script
{
    // 连接的发起方，保持连接。每隔一段时间检查一下
    public class CheckConnections_Loop<S> : Handler<S>
        where S : Service
    {
        public const int INTERVAL_S_QUICK = 1;
        public const int INTERVAL_S_SLOW = 31;
        public override MsgType msgType => MsgType._CheckConnections_Loop;

        public override async Task<MyResponse> Handle(ProtocolClientData _socket/* null */, object _msg/* null */)
        {
            if (this.service.data.connectToServiceTypes.Count > 0)
            {
                return await this.service.connectToSelf.SendToSelfAsync(MsgType._CheckConnections, null);
            }

            return new MyResponse(ECode.Success, new ResCheckConnections());
        }

        public override MyResponse PostHandle(ProtocolClientData socket, object msg, MyResponse r)
        {
            if (this.service.data.state >= ServiceState.ShuttingDown)
            {
                return r;
            }

            this.service.data.timer_CheckConnections_Loop = this.server.timerScript.SetTimer(this.service.serviceId, this.service.data.state == ServiceState.Started ? INTERVAL_S_SLOW : INTERVAL_S_QUICK, this.msgType, null);
            return r;
        }
    }
}