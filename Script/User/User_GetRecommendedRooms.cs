using Data;

namespace Script
{
    [AutoRegister]
    public class User_GetRecommendedRooms : Handler<UserService, MsgGetRecommendedRooms, ResGetRecommendedRooms>
    {
        public User_GetRecommendedRooms(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType.GetRecommendedRooms;

        public override async Task<ECode> Handle(MessageContext context, MsgGetRecommendedRooms msg, ResGetRecommendedRooms res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            var msgDb = new MsgSearch_RoomInfo();
            msgDb.keyword = "apartment";

            var r = await this.service.dbServiceProxy.Search_RoomInfo(msgDb);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var resDb = r.CastRes<ResSearch_RoomInfo>();

            res.roomInfos = resDb.roomInfos;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgGetRecommendedRooms msg, ECode e, ResGetRecommendedRooms res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}