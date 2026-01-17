using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<ECode> SetName(string name)
        {
            this.Log($"SetName {name}");

            var msg = new MsgSetName();
            msg.userName = name;

            var r = await this.connection.Request(MsgType.SetName, msg);
            this.Log($"SetName result {r.e}");
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var res = r.CastRes<ResSetName>();
            return ECode.Success;
        }
    }
}