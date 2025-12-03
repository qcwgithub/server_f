using Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Script
{
    public class Database_QueryUser_byId : Handler<DatabaseService>
    {
        public override MsgType msgType => MsgType._Database_QueryUser_byId;
        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQueryUserById>(_msg);
            MyResponse r = await this.service.collection_user.QueryById(msg.userId);
            if (r.err != ECode.Success)
            {
                return r;
            }

            var res = new ResQueryUserById();
            res.userId = msg.userId;
            res.list = new List<Profile>();
            res.list.AddRange(r.CastRes<List<Profile>>());

            return new MyResponse(ECode.Success, res);
        }
    }
}