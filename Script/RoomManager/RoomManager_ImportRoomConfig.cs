using Data;

namespace Script
{
    [AutoRegister]
    public class RoomManager_ImportRoomConfig : Handler<RoomManagerService, MsgRoomManagerImportRoomConfig, ResRoomManagerImportRoomConfig>
    {
        public RoomManager_ImportRoomConfig(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._RoomManager_ImportRoomConfig;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomManagerImportRoomConfig msg, ResRoomManagerImportRoomConfig res)
        {
            this.service.logger.Info($"{this.msgType} file {msg.file}");

            if (string.IsNullOrEmpty(msg.file) || !File.Exists(msg.file))
            {
                return ECode.FileNotExist;
            }

            string text = ConfigLoader.ReadAllText(msg.file);

            var helper = CsvUtils.Parse(text);
            int successCount = 0;
            while (helper.ReadRow())
            {
                string title = helper.ReadString(nameof(title));
                string desc = helper.ReadString(nameof(desc));

                long roomId = this.service.roomIdSnowflakeScript.NextRoomId();
                SceneInfo sceneInfo = this.service.roomScript.NewSceneInfo(roomId);
                sceneInfo.title = title;
                sceneInfo.desc = desc;

                ECode e = await this.service.roomScript.InsertSceneInfo(sceneInfo);
                if (e == ECode.Success)
                {
                    successCount++;
                }
            }

            this.service.logger.Info($"{this.msgType} successCount {successCount}");

            return ECode.Success;
        }
    }
}