using Data;

namespace Script
{
    [AutoRegister]
    public class User_SendSceneChat : Handler<UserService, MsgSendSceneChat, ResSendSceneChat>
    {
        public override MsgType msgType => MsgType.SendSceneChat;
        public User_SendSceneChat(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSendSceneChat msg, ResSendSceneChat res)
        {
            ECode e = RoomUtils.CheckRoomId(msg.roomId);
            if (e != ECode.Success)
            {
                return ECode.InvalidRoomId;
            }

            e = ChatUtils.CheckChatMessageType(msg.chatMessageType);
            if (e != ECode.Success)
            {
                return e;
            }

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            e = this.service.roomScript.CheckSendSceneChat(user, msg);
            if (e != ECode.Success)
            {
                return e;
            }

            UserInfo userInfo = user.userInfo;

            MyResponse r;

            stObjectLocation location = await this.service.roomLocator.GetLocation(msg.roomId);
            if (!location.IsValid())
            {
                return ECode.RoomLocationNotExist;
            }

            var msgR = new MsgRoomSendSceneChat();
            msgR.roomId = msg.roomId;
            msgR.userId = user.userId;
            msgR.type = msg.chatMessageType;
            msgR.content = msg.content;
            msgR.userName = user.userInfo.userName;
            msgR.avatarIndex = user.userInfo.avatarIndex;
            msgR.clientMessageId = msg.clientMessageId;
            msgR.imageContent = msg.imageContent;

            r = await this.service.roomServiceProxy.SendSceneChat(location.serviceId, msgR);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var resR = r.CastRes<ResRoomSendSceneChat>();

            await Task.Delay(1000);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSendSceneChat msg, ECode e, ResSendSceneChat res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}