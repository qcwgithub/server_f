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

        public CheckConnections_Loop(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Service_CheckConnections_Loop;

        public override async Task<ECode> Handle(MessageContext context, MsgCheckConnections_Loop msg, ResCheckConnections_Loop res)
        {
            if (this.service.data.connectToServiceTypes.Count > 0)
            {
                return await this.service.CheckConnections();
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, object msg, MyResponse r)
        {
            if (this.service.data.state >= ServiceState.ShuttingDown)
            {
                return;
            }

            this.service.data.timer_CheckConnections_Loop = this.server.timerScript.SetTimer(this.service.serviceId, this.service.data.state == ServiceState.Started ? INTERVAL_S_SLOW : INTERVAL_S_QUICK, this.msgType, null);
        }
    }
}