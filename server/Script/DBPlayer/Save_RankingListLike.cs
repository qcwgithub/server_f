using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_RankingListLike : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_RankingListLike;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_RankingListLike>(_msg);
            this.service.logger.InfoFormat("{0} rankName {1} memberId {2}", this.msgType, msg.info.rankName, msg.info.memberId);

            ECode e = await this.service.collection_raking_list_like.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_RankingListLike();
            return new MyResponse(ECode.Success, res);
        }
    }
}
