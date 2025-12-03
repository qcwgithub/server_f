using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileRankingPromotion_all : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_ProfileRankingPromotion_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileRankingPromotion_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_profile_ranking_promotion.Query_ProfileRankingPromotion_all();

            var res = new ResQuery_ProfileRankingPromotion_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
