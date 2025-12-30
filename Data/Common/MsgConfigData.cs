namespace Data
{
    public struct stMsgConfig
    {
        public MsgQueue queue;
    }

    public partial class MsgConfigData
    {
        public readonly Dictionary<MsgType, stMsgConfig> configDict;
        public MsgConfigData()
        {
            this.configDict = new();
            this.Init();
        }

        public stMsgConfig GetMsgConfig(MsgType msgType)
        {
            return this.configDict.TryGetValue(msgType, out stMsgConfig config) ? config : default;
        }
    }
}