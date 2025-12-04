using System.Collections.Generic;
using System.Net.Sockets;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSimulatePrepareUserLogin
    {
        [Key(0)]
        public long userId;
    }

    [MessagePackObject]
    public class ResSimulatePrepareUserLogin
    {

    }
}