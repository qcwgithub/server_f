using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_UnionCUnionInfo : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_UnionCUnionInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_UnionCUnionInfo>(_msg);
            this.service.logger.InfoFormat("{0} unionId {1}", this.msgType, msg.info.unionId);

            ECode e = await this.service.collection_unionc_union_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_UnionCUnionInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
