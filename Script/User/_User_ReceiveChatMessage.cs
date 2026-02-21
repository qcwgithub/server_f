using Data;

namespace Script
{
    // 别人给我发送消息
    [AutoRegister]
    public class _User_ReceiveChatMessage : Handler<UserService, MsgReceiveChatMessage, ResReceiveChatMessage>
    {
        public override MsgType msgType => MsgType._User_ReceiveChatMessage;
        public _User_ReceiveChatMessage(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgReceiveChatMessage msg, ResReceiveChatMessage res)
        {
            this.service.logger.Info($"{this.msgType} userId {msg.userId}");

            User? user = await this.service.LockUser(msg.userId, context);
            if (user == null)
            {
                return ECode.Offline;
            }

            if (user.connection != null)
            {
                user.connection.Send(MsgType.AChatMessage, new MsgAChatMessage
                {
                    message = msg.message
                }, null);
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgReceiveChatMessage msg, ECode e, ResReceiveChatMessage res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}