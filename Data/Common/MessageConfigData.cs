namespace Data
{
    public struct stMessageConfig
    {
        public MessageQueue queue;
    }

    public partial class MessageConfigData
    {
        public readonly Dictionary<MsgType, stMessageConfig> configDict;
        public MessageConfigData()
        {
            this.configDict = new();
            this.InitDict();
        }

        public stMessageConfig GetMsgConfig(MsgType msgType)
        {
            return this.configDict.TryGetValue(msgType, out stMessageConfig config) ? config : default;
        }
    }
}