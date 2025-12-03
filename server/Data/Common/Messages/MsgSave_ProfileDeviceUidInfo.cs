using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_ProfileDeviceUidInfo
    {
        [Key(0)]
        public ProfileDeviceUidInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_ProfileDeviceUidInfo
    {
    }
}
