using Data;
using System.Threading.Tasks;

namespace Script
{
    public class Database_InsertUser : Handler<DatabaseService>
    {
        public override MsgType msgType => MsgType._Database_InsertUser;
        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgInsertUser>(_msg);

            this.service.logger.InfoFormat("{0}, userId: {1}", this.msgType, msg.userId);

            MyResponse r = await this.service.collection_user.Insert(msg.userId, msg.profile);
            return r;
        }
    }
}