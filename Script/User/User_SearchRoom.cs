using Data;

namespace Script
{
    public class User_SearchRoom : Handler<UserService, MsgSearchRoom, ResSearchRoom>
    {
        public User_SearchRoom(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType.SearchRoom;

        public override async Task<ECode> Handle(MessageContext context, MsgSearchRoom msg, ResSearchRoom res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} keyword {msg.keyword}");
            if (string.IsNullOrEmpty(msg.keyword))
            {
                return ECode.InvalidParam;
            }

            if (msg.keyword.Length > 100)
            {
                return ECode.InvalidParam;
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

        public override void PostHandle(MessageContext context, MsgSearchRoom msg, ECode e, ResSearchRoom res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}