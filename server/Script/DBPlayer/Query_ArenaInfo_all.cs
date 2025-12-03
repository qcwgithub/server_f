using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ArenaInfo_all : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_ArenaInfo_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ArenaInfo_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_arena_info.Query_ArenaInfo_all();

            var res = new ResQuery_ArenaInfo_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
