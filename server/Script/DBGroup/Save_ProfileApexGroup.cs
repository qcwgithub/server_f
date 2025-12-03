using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ProfileApexGroup : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_ProfileApexGroup;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ProfileApexGroup>(_msg);
            this.service.logger.InfoFormat("{0} groupId {1}", this.msgType, msg.info.groupId);

            ECode e = await this.service.collection_profile_apex_group.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ProfileApexGroup();
            return new MyResponse(ECode.Success, res);
        }
    }
}
