using Data;
using System.Threading.Tasks;

namespace Script
{
    public class Db_InsertUserProfile : Handler<DbService, MsgInsertUserProfile>
    {
        public override MsgType msgType => MsgType._Db_InsertUserProfile;
        public override async Task<MyResponse> Handle(ProtocolClientData socket, MsgInsertUserProfile msg)
        {
            this.service.logger.InfoFormat("{0}, userId: {1}", this.msgType, msg.profile.userId);

            MyResponse r = await this.service.collection_user_profile.Insert(msg.profile);
            return r;
        }
    }
}