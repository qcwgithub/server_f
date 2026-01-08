using Data;

namespace Script
{
    public class Search_RoomInfo : Handler<DbService, MsgSearch_RoomInfo, ResSearch_RoomInfo>
    {
        public Search_RoomInfo(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Search_RoomInfo;
        public override async Task<ECode> Handle(MessageContext context, MsgSearch_RoomInfo msg, ResSearch_RoomInfo res)
        {
            this.service.logger.Info($"{this.msgType} keyword {msg.keyword}");

            res.roomInfos = await this.service.collection_room_info.Search(msg.keyword);
            return ECode.Success;
        }
    }
}