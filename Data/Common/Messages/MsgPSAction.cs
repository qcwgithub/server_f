using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgPSAction
    {
        [Key(0)]
        public bool? allowNewPlayer; // false 表示 AAA 不会分配新玩家到此 PlayerService
        [Key(1)]
        public int? playerDestroyTimeoutS; // 下线后多久清除此玩家
        [Key(2)]
        public int? playerSaveIntervalS;
        [Key(3)]
        public bool? printTALogComsumerBufferSize;
        [Key(4)]
        public bool? clearTALogConsumerBuffer;
    }
}