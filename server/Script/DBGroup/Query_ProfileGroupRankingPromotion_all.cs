using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileGroupRankingPromotion_all : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileGroupRankingPromotion_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileGroupRankingPromotion_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_group_ranking_promotion.Query_ProfileGroupRankingPromotion_all();

            var res = new ResQuery_ProfileGroupRankingPromotion_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
