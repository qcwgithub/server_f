using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgPrepareUserLogin
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public string token;
        [Key(2)]
        public AccountInfo accountInfo;
        [Key(3)]
        public string userName;
        [Key(4)]
        public string platform;
        [Key(5)]
        public string verifyData;
        [Key(6)]
        public string accountDetail;
        [Key(7)]
        public string oaid;
        [Key(8)]
        public string imei;
        [Key(9)]
        public string ip;
        [Key(10)]
        public string version;

        [Key(11)]
        public int timezoneOffset; // 分钟，北京时间此值是480 
        [Key(12)]
        public string deviceUid;
        [Key(13)]
        public string deviceModel;
        [Key(14)]
        public string osVersion;

        [IgnoreMember]
        [JsonIgnore]
        public string channel 
        {
            get
            {
                return this.accountInfo.channel;
            }
        }
        [IgnoreMember]
        [JsonIgnore]
        public string channelUserId
        {
            get
            {
                return this.accountInfo.channelUserId;
            }
        }
    }
    [MessagePackObject]
    public class ResPreparePlayerLogin
    {
        [Key(0)]
        public int playerCount;
    }
}