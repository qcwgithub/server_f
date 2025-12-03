using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_listOf_UnionBrawlUnionSeasonInfo_unionId_all : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_listOf_UnionBrawlUnionSeasonInfo_unionId_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_listOf_UnionBrawlUnionSeasonInfo_unionId_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_union_brawl_union_season_info.Query_listOf_UnionBrawlUnionSeasonInfo_unionId_all();

            var res = new ResQuery_listOf_UnionBrawlUnionSeasonInfo_unionId_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
