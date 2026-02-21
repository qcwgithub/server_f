using Data;

namespace Script
{
    public partial class RoomService
    {
        public async Task<ECode> SaveSceneInfo(Room room, string reason)
        {
            await this.server.roomLocationRedisW.WriteLocation(room.roomId, this.serviceId, this.sd.saveIntervalS + 60);

            var msgDb = new MsgSave_SceneInfo
            {
                roomId = room.roomId,
                sceneInfoNullable = new SceneInfoNullable()
            };
            var infoNullable = msgDb.sceneInfoNullable;

            List<string>? buffer = null;
            if (room.lastSceneInfo == null)
            {
                this.logger.Error($"SaveRoom room.lastSceneInfo == null");
                return ECode.Error;
            }

            SceneInfo last = room.lastSceneInfo;
            SceneInfo curr = room.sceneInfo;

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
            if (last.messageId != curr.messageId)
            {
                infoNullable.messageId = curr.messageId;
                last.messageId = curr.messageId;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("messageId");
            }
            if (last.participants.IsDifferent_ListClass(curr.participants))
            {
                infoNullable.participants = curr.participants;
                last.participants.DeepCopyFrom_ListClass(curr.participants);
                if (buffer == null) buffer = new List<string>();
                buffer.Add("participants");
            }

            #endregion auto

            // player.lastSceneInfo = curr; // 先假设一定成功吧
            if (last.IsDifferent(curr))
            {
                this.logger.Error("last.IsDifferent(curr)!!!");
            }

            string fieldsStr = "";
            if (buffer != null)
            {
                fieldsStr = string.Join(", ", buffer.ToArray());

                this.logger.InfoFormat("SaveRoom roomId {0}, reason {1}, fields [{2}]", room.roomId, reason, fieldsStr);
            }

            if (buffer != null)
            {
#if DEBUG
                msgDb.sceneInfo_debug = SceneInfo.Ensure(null);
                msgDb.sceneInfo_debug.DeepCopyFrom(curr);
#endif
                var r = await this.dbServiceProxy.Save_SceneInfo(msgDb);
                if (r.e != ECode.Success)
                {
                    this.logger.ErrorFormat("{0} error: {1}, roomId {2}", MsgType._Save_SceneInfo, r.e, room.roomId);
                    return r.e;
                }
            }

            return ECode.Success;
        }
    }
}