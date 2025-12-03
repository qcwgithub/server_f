using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Iterate_listOf_RankingListLike_by_memberId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Iterate_listOf_RankingListLike_by_memberId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgIterate_listOf_RankingListLike_by_memberId>(_msg);
            // this.service.logger.InfoFormat("{0} start_memberId: {1}, end_memberId: {2}", this.msgType, msg.start_memberId, msg.end_memberId);

            var result = await this.service.collection_raking_list_like.Iterate_listOf_RankingListLike_by_memberId(msg.start_memberId, msg.end_memberId);

            var res = new ResIterate_listOf_RankingListLike_by_memberId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
