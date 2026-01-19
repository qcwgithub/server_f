using System.ComponentModel;
using Data;

namespace Script
{
    public class User_ReportRoomMessage : Handler<UserService, MsgReportRoomMessage, ResReportRoomMessage>
    {
        public override MsgType msgType => MsgType.ReportRoomMessage;
        public User_ReportRoomMessage(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgReportRoomMessage msg, ResReportRoomMessage res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} roomId {msg.roomId} messageId {msg.messageId} reason {msg.reason}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }
            if (msg.roomId <= 0)
            {
                return ECode.InvalidRoomId;
            }

            if (msg.roomId != user.roomId)
            {
                return ECode.WrongRoomId;
            }

            if (msg.reason < 0 || msg.reason >= RoomMessageReportReason.Count)
            {
                return ECode.InvalidParam;
            }

            if (msg.messageId <= 0)
            {
                return ECode.InvalidParam;
            }

            ChatMessage? message = await this.server.roomMessagesRedis.QueryOne(msg.roomId, msg.messageId);

            var info = new RoomMessageReportInfo();
            info.reportUserId = user.userId;
            info.targetUserId = message == null ? 0 : message.senderId;
            info.roomId = user.roomId;
            info.messageId = msg.messageId;
            info.reason = msg.reason;
            info.timeS = TimeUtils.GetTimeS();

            var msgDb = new MsgSave_RoomMessageReportInfo();
            msgDb.info = info;

            await this.service.dbServiceProxy.Save_RoomMessageReportInfo(msgDb);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgReportRoomMessage msg, ECode e, ResReportRoomMessage res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}