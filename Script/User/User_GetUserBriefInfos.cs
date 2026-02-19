using Data;

namespace Script
{
    [AutoRegister]
    public class User_GetUserBriefInfos : Handler<UserService, MsgGetUserBriefInfos, ResGetUserBriefInfos>
    {
        public override MsgType msgType => MsgType.GetUserBriefInfos;
        public User_GetUserBriefInfos(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgGetUserBriefInfos msg, ResGetUserBriefInfos res)
        {
            if (msg.userIds == null || msg.userIds.Count == 0)
            {
                return ECode.InvalidParam;
            }

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            var idList = msg.userIds.ToList();
            res.userBriefInfos = await this.server.userBriefInfoProxy.GetMany(this.service.dbServiceProxy, idList);
            for (int i = 0; i < res.userBriefInfos.Count; i++)
            {
                if (res.userBriefInfos[i] == null)
                {
                    var temp = UserBriefInfo.Ensure(null);
                    temp.userId = idList[i];
                    temp.avatarIndex = 0;
                    temp.userName = "User_" + idList[i];
                    res.userBriefInfos[i] = temp;
                }
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgGetUserBriefInfos msg, ECode e, ResGetUserBriefInfos res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}