using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PersistenceTaskQueueHandler_Loop : TickLoopHandler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._PersistenceTaskQueueHandler_Loop;
        protected override MsgType childMsgType => MsgType._PersistenceTaskQueueHandler;
        protected override object childMsg => null;
        protected override int intervalS => 1;
        
        protected override void SaveTimer(ITimer timer)
        {
            this.service.dbPlayerData.timer_persistence_taskQueueHandler_Loop = timer;
        }
    }
}