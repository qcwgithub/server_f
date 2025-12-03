using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ProfileDeviceUidInfo_by_deviceUid
    {
        [Key(0)]
        public string deviceUid;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ProfileDeviceUidInfo_by_deviceUid
    {
        [Key(0)]
        public ProfileDeviceUidInfo result;
    }
}
