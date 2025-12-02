using MessagePack;
using System.Collections.Generic;
namespace Data
{
    [MessagePackObject]
    public class MsgCommon
    {
        [Key(0)]
        public Dictionary<string, long> longDict;
        [Key(1)]
        public Dictionary<string, string> stringDict;

        public long GetLong(string key)
        {
            return this.longDict[key];
        }

        public string GetString(string key)
        {
            return this.stringDict[key];
        }

        public MsgCommon SetLong(string key, long v)
        {
            if (this.longDict == null)
            {
                this.longDict = new Dictionary<string, long>();
            }
            this.longDict[key] = v;
            return this;
        }
        public MsgCommon SetString(string key, string v)
        {
            if (this.stringDict == null)
            {
                this.stringDict = new Dictionary<string, string>();
            }
            this.stringDict[key] = v;
            return this;
        }
    }
}