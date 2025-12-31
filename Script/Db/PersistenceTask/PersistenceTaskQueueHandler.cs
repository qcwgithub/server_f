using Data;

namespace Script
{
    public partial class PersistenceTaskQueueHandler : PersistenceTaskQueueHandler_Base<DbService>
    {
        public override MsgType msgType => MsgType._Service_PersistenceTaskQueueHandler;
        protected override LockController lockController => this.service.lockController;

        protected override string key_PersistenceTaskQueueOwners
        {
            get => DbKey.PersistenceTaskQueueOwners();
        }
        protected override bool persistenceHandling
        {
            get => this.service.sd.persistenceHandling;
            set => this.service.sd.persistenceHandling = value;
        }
        protected override long persistence_lastAssignTaskQueueOwnersTimeS
        {
            get => this.service.sd.persistence_lastAssignTaskQueueOwnersTimeS;
            set => this.service.sd.persistence_lastAssignTaskQueueOwnersTimeS = value;
        }
        protected override List<int> persistence_ownTaskQueues
        {
            get => this.service.sd.persistence_ownTaskQueues;
            set => this.service.sd.persistence_ownTaskQueues = value;
        }
        protected override string lockKey_AssignPersistenceTaskQueueOwners
        {
            get => DbKey.LockKey.AssignPersistenceTaskQueueOwners();
        }
        protected override PersistenceTaskQueueRedis taskQueueRedis
        {
            get => this.server.persistence_taskQueueRedis;
        }

        public PersistenceTaskQueueHandler(Server server, DbService service) : base(server, service)
        {

        }

        protected override async Task<stPersistenceResult> HandleTask(int taskQueue, string dirtyElement)
        {
            // this.service.logger.InfoFormat("{0} taskQueue {1} save {2}", this.msgType, taskQueue, dirtyElement);

            ECode err;
            bool putBack = false;

            stDirtyElement element = stDirtyElement.FromString(dirtyElement);

            switch (element.e)
            {
                #region auto_callSave

                case DirtyElementType.AccountInfo:
                    (err, putBack) = await this.SaveAccountInfo(element);
                    break;


                #endregion auto_callSave

                default:
                    this.service.logger.ErrorFormat("{0} unknown element {1}", this.msgType, element.ToString());
                    err = ECode.Error;
                    putBack = true;
                    break;
            }

            var result = new stPersistenceResult();
            result.taskQueue = taskQueue;
            result.dirtyElement = dirtyElement;
            result.e = err;
            result.putBack = putBack;
            return result;
        }
    }
}