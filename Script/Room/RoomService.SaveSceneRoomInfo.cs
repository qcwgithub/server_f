using Data;

namespace Script
{
    public partial class RoomService
    {
        public async Task<ECode> SaveSceneRoomInfo(SceneRoom sceneRoom, string reason)
        {
            await this.server.roomLocationRedisW.WriteLocation(sceneRoom.roomId, this.serviceId, this.sd.saveIntervalS + 60);

            var msgDb = new MsgSave_SceneRoomInfo
            {
                roomId = sceneRoom.roomId,
                roomInfoNullable = new SceneRoomInfoNullable()
            };
            var infoNullable = msgDb.roomInfoNullable;

            List<string>? buffer = null;
            if (sceneRoom.lastSceneRoomInfo == null)
            {
                this.logger.Error($"SaveRoom room.lastSceneRoomInfo == null");
                return ECode.Error;
            }

            SceneRoomInfo last = sceneRoom.lastSceneRoomInfo;
            SceneRoomInfo curr = sceneRoom.roomInfo;

            #region auto

            if (last.roomId != curr.roomId)
            {
                infoNullable.roomId = curr.roomId;
                last.roomId = curr.roomId;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("roomId");
            }
            if (last.createTimeS != curr.createTimeS)
            {
                infoNullable.createTimeS = curr.createTimeS;
                last.createTimeS = curr.createTimeS;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("createTimeS");
            }
            if (last.title != curr.title)
            {
                infoNullable.title = curr.title;
                last.title = curr.title;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("title");
            }
            if (last.desc != curr.desc)
            {
                infoNullable.desc = curr.desc;
                last.desc = curr.desc;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("desc");
            }
            if (last.messageSeq != curr.messageSeq)
            {
                infoNullable.messageSeq = curr.messageSeq;
                last.messageSeq = curr.messageSeq;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("messageSeq");
            }

            #endregion auto

            // player.lastSceneRoomInfo = curr; // 先假设一定成功吧
            if (last.IsDifferent(curr))
            {
                this.logger.Error("last.IsDifferent(curr)!!!");
            }

            string fieldsStr = "";
            if (buffer != null)
            {
                fieldsStr = string.Join(", ", buffer.ToArray());

                this.logger.InfoFormat("SaveRoom roomId {0}, reason {1}, fields [{2}]", sceneRoom.roomId, reason, fieldsStr);
            }

            if (buffer != null)
            {
#if DEBUG
                msgDb.roomInfo_debug = SceneRoomInfo.Ensure(null);
                msgDb.roomInfo_debug.DeepCopyFrom(curr);
#endif
                var r = await this.dbServiceProxy.Save_SceneRoomInfo(msgDb);
                if (r.e != ECode.Success)
                {
                    this.logger.ErrorFormat("{0} error: {1}, roomId {2}", MsgType._Save_SceneRoomInfo, r.e, sceneRoom.roomId);
                    return r.e;
                }
            }

            return ECode.Success;
        }
    }
}