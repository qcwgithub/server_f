using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ProfileGroupUnionBrawl : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_ProfileGroupUnionBrawl;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ProfileGroupUnionBrawl>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_profile_group_union_brawl.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ProfileGroupUnionBrawl();
            return new MyResponse(ECode.Success, res);
        }
    }
}
