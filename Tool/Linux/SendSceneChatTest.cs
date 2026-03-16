using Data;

namespace Tool
{
    public partial class LinuxProgram
    {
        async Task SendSceneChatTest()
        {
            string roomIdStr = AskHelp.AskInput("room id?").OnAnswer();
            long roomId = long.Parse(roomIdStr);

            string countStr = AskHelp.AskInput("count?").OnAnswer();
            int count = int.Parse(countStr);

            List<ServiceConfig> serviceConfigs = this.SelectServices("Select Room Manager?", true);
            var msgLoad = new MsgRoomManagerLoadRoom();
            msgLoad.roomId = roomId;

            var r = await this.Connect_Request_Close(serviceConfigs[0], MsgType._RoomManager_LoadRoom, msgLoad);
            if (r.e != ECode.Success)
            {
                return;
            }

            var resLoad = r.CastRes<ResRoomManagerLoadRoom>();
            var location = resLoad.location;

            serviceConfigs = this.SelectServices($"Select Room(serviceId = {location.serviceId})?", true);

            var msgTest = new MsgRoomSendSceneChatTest();
            msgTest.roomId = roomId;
            msgTest.count = count;

            r = await this.Connect_Request_Close(serviceConfigs[0], MsgType._Room_SendSceneChatTest, msgTest);
            if (r.e != ECode.Success)
            {
                return;
            }
        }
    }
}