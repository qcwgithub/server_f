using Data;
using System.Threading.Tasks;

namespace Script
{
    // 连接的发起方，保持连接。每隔一段时间检查一下
    public class CheckConnections_Loop<S> : Handler<S, MsgCheckConnections_Loop, ResCheckConnections_Loop>
        where S : Service
    {
        public const int INTERVAL_S_QUICK = 1;
        public const int INTERVAL_S_SLOW = 31;
        public override MsgType msgType => MsgType._CheckConnections_Loop;

        public override async Task<ECode> Handle(ProtocolClientData _socket, MsgCheckConnections_Loop msg, ResCheckConnections_Loop res)
        {
            if (this.service.data.connectToServiceTypes.Count > 0)
            {
                var r = await this.service.connectToSelf.Send<MsgCheckConnections, ResCheckConnections>(MsgType._CheckConnections, new MsgCheckConnections());
                return r.e;
            }

            return ECode.Success;
        }

        public override (ECode, object) PostHandle(ProtocolClientData socket, object msg, ECode e, object res)
        {
            if (this.service.data.state >= ServiceState.ShuttingDown)
            {
                return (e, res);
            }

            this.service.data.timer_CheckConnections_Loop = this.server.timerScript.SetTimer(this.service.serviceId, this.service.data.state == ServiceState.Started ? INTERVAL_S_SLOW : INTERVAL_S_QUICK, this.msgType, null);
            return (e, res);
        }
    }
}