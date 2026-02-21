using Data;

namespace Tool
{
    public partial class LinuxProgram
    {
        async Task SearchScene()
        {
            List<ServiceConfig> serviceConfigs = this.SelectServices("Select User?", true);
            while (true)
            {
                string keyword = AskHelp.AskInput("keyword(empty to exit)?").OnAnswer();
                if (string.IsNullOrEmpty(keyword))
                {
                    break;
                }

                var msg = new MsgSearchScene { keyword = keyword };
                var r = await this.Connect_Request_Close(serviceConfigs[0], MsgType.SearchScene, msg);
                if (r.e == ECode.Success)
                {
                    var res = r.CastRes<ResSearchScene>();
                    Console.WriteLine(JsonUtils.stringifyIndent(res.sceneRoomInfos));
                }
            }
        }
    }
}