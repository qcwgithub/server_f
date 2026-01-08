using Data;

namespace Tool
{
    public partial class LinuxProgram
    {
        async Task SearchRoom()
        {
            List<ServiceConfig> serviceConfigs = this.SelectServices("Select User?", true);
            while (true)
            {
                string keyword = AskHelp.AskInput("keyword(empty to exit)?").OnAnswer();
                if (string.IsNullOrEmpty(keyword))
                {
                    break;
                }

                var msg = new MsgSearchRoom { keyword = keyword };
                var r = await this.Connect_Request_Close(serviceConfigs[0], MsgType.SearchRoom, msg);
                if (r.e == ECode.Success)
                {
                    var res = r.CastRes<ResSearchRoom>();
                    Console.WriteLine(JsonUtils.stringifyIndent(res.roomInfos));
                }
            }
        }
    }
}