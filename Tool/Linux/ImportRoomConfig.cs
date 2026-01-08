using Data;

namespace Tool
{
    public partial class LinuxProgram
    {
        async Task ImportRoomConfig()
        {
            List<ServiceConfig> serviceConfigs = this.SelectServices(null, true);
            string file = AskHelp.AskInput("file?").OnAnswer();
            if (string.IsNullOrEmpty(file))
            {
                return;
            }

            var msg = new MsgRoomManagerImportRoomConfig { file = file };
            await this.Connect_Request_Close(serviceConfigs, MsgType._RoomManager_ImportRoomConfig, msg);
        }
    }
}