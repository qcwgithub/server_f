using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ChampionGroupRobotInfo : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_ChampionGroupRobotInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ChampionGroupRobotInfo>(_msg);
            this.service.logger.InfoFormat("{0} groupType {1} groupId {2}", this.msgType, msg.info.groupType, msg.info.groupId);

            ECode e = await this.service.collection_champion_group_robot_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ChampionGroupRobotInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
