using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_UnionSeasonInfo : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_UnionSeasonInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_UnionSeasonInfo>(_msg);
            this.service.logger.InfoFormat("{0} unionId {1}", this.msgType, msg.info.unionId);

            ECode e = await this.service.collection_union_season_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_UnionSeasonInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
