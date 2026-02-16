using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_UserBriefInfo : Handler<DbService, MsgSave_UserBriefInfo, ResSave_UserBriefInfo>
    {
        public override MsgType msgType => MsgType._Save_UserBriefInfo;

        public Save_UserBriefInfo(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSave_UserBriefInfo msg, ResSave_UserBriefInfo res)
        {
            this.service.logger.InfoFormat("{0} userId {1}", this.msgType, msg.info.userId);

            ECode e = await this.service.collection_user_brief_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}
