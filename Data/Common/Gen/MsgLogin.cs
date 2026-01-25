using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgLogin
    {
        [Key(0)]
        public string version;
        [Key(1)]
        public string platform;
        [Key(2)]
        public string channel;
        [Key(3)]
        public string channelUserId;
        [Key(4)]
        public string verifyData;
        [Key(5)]
        public long userId;
        [Key(6)]
        public string token;
        [Key(7)]
        public string deviceUid;
        [Key(8)]
        public Dictionary<string, string> dict;

        public static MsgLogin Ensure(MsgLogin? p)
        {
            if (p == null)
            {
                p = new MsgLogin();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.version == null)
            {
                this.version = string.Empty;
            }
            if (this.platform == null)
            {
                this.platform = string.Empty;
            }
            if (this.channel == null)
            {
                this.channel = string.Empty;
            }
            if (this.channelUserId == null)
            {
                this.channelUserId = string.Empty;
            }
            if (this.verifyData == null)
            {
                this.verifyData = string.Empty;
            }
            if (this.token == null)
            {
                this.token = string.Empty;
            }
            if (this.deviceUid == null)
            {
                this.deviceUid = string.Empty;
            }
            if (this.dict == null)
            {
                this.dict = new Dictionary<string, string>();
            }
        }

        public bool IsDifferent(MsgLogin other)
        {
            if (this.version != other.version)
            {
                return true;
            }
            if (this.platform != other.platform)
            {
                return true;
            }
            if (this.channel != other.channel)
            {
                return true;
            }
            if (this.channelUserId != other.channelUserId)
            {
                return true;
            }
            if (this.verifyData != other.verifyData)
            {
                return true;
            }
            if (this.userId != other.userId)
            {
                return true;
            }
            if (this.token != other.token)
            {
                return true;
            }
            if (this.deviceUid != other.deviceUid)
            {
                return true;
            }
            if (this.dict.IsDifferent_DictValue(other.dict))
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(MsgLogin other)
        {
            this.version = other.version;
            this.platform = other.platform;
            this.channel = other.channel;
            this.channelUserId = other.channelUserId;
            this.verifyData = other.verifyData;
            this.userId = other.userId;
            this.token = other.token;
            this.deviceUid = other.deviceUid;
            this.dict.DeepCopyFrom_DictValue(other.dict);
        }
    }
}
