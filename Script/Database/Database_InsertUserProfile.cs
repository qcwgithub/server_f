using Data;
using System.Threading.Tasks;

namespace Script
{
    public class Database_InsertUserProfile : Handler<DatabaseService>
    {
        public override MsgType msgType => MsgType._Database_InsertUserProfile;
        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgInsertUserProfile>(_msg);

            this.service.logger.InfoFormat("{0}, userId: {1}", this.msgType, msg.profile.userId);

            MyResponse r = await this.service.collection_user_profile.Insert(msg.profile);
            return r;
        }
    }
}