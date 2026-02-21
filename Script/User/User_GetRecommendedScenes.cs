using Data;

namespace Script
{
    [AutoRegister]
    public class User_GetRecommendedScenes : Handler<UserService, MsgGetRecommendedScenes, ResGetRecommendedScenes>
    {
        public User_GetRecommendedScenes(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType.GetRecommendedScenes;

        public override async Task<ECode> Handle(MessageContext context, MsgGetRecommendedScenes msg, ResGetRecommendedScenes res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            var msgDb = new MsgSearch_SceneInfo();
            msgDb.keyword = "apartment";

            var r = await this.service.dbServiceProxy.Search_SceneInfo(msgDb);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var resDb = r.CastRes<ResSearch_SceneInfo>();

            res.sceneInfos = resDb.sceneInfos;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgGetRecommendedScenes msg, ECode e, ResGetRecommendedScenes res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}