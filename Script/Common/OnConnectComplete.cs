using Data;

namespace Script
{
    // 连接其他服务器成功
    public class OnConnectComplete<S> : Handler<S, MsgOnConnectComplete, ResOnConnectComplete>
        where S : Service
    {
        public OnConnectComplete(Server server, S service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._OnConnectComplete;

        public override async Task<ECode> Handle(MsgContext context, MsgOnConnectComplete msg, ResOnConnectComplete res)
        {
            if (context.connection is ServiceConnection serviceConnection)
            {

                this.logger.InfoFormat("{0} connection id: {1}, to: {2}", this.msgType, serviceConnection.GetConnectionId(), serviceConnection.tai.ToString());

                // 连上去之后立即向他报告是我的身份
                var msgInfo = new MsgConnectorInfo();
                msgInfo.connectorInfo = this.service.CreateConnectorInfo();
                await serviceConnection.Request<MsgConnectorInfo, ResConnectorInfo>(MsgType._ConnectorInfo, msgInfo);
            }

            return ECode.Success;
        }
    }
}