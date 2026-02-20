using Data;

namespace Script
{
    [AutoRegister]
    public class User_SearchScene : Handler<UserService, MsgSearchScene, ResSearchScene>
    {
        public User_SearchScene(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType.SearchScene;

        public override async Task<ECode> Handle(MessageContext context, MsgSearchScene msg, ResSearchScene res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} keyword {msg.keyword}");
            if (string.IsNullOrEmpty(msg.keyword))
            {
                return ECode.SearchTooShort;
            }

            if (msg.keyword.Length > 100)
            {
                return ECode.SearchTooLong;
            }

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            var msgDb = new MsgSearch_RoomInfo();
            msgDb.keyword = msg.keyword;

            var r = await this.service.dbServiceProxy.Search_RoomInfo(msgDb);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var resDb = r.CastRes<ResSearch_RoomInfo>();

            res.roomInfos = resDb.roomInfos;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSearchScene msg, ECode e, ResSearchScene res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}