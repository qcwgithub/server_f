using Data;

namespace Script
{
    public partial class DbService : Service
    {
        public DbServiceData sd
        {
            get
            {
                return (DbServiceData)this.data;
            }
        }

        public readonly LockController lockController;
        public readonly GlobalServiceProxy globalServiceProxy;
        readonly TaskQueueOwnersRedis taskQueueOwnersRedis;

        #region auto_collection_var_decl

        public collection_user_info collection_user_info;
        public collection_account_info collection_account_info;
        public collection_scene_info collection_scene_info;
        public collection_message_report_info collection_message_report_info;
        public collection_user_report_info collection_user_report_info;
        public collection_user_brief_info collection_user_brief_info;
        public collection_private_room_info collection_private_room_info;

        #endregion auto_collection_var_decl

        public DbService(Server server, int serviceId) : base(server, serviceId)
        {
            this.lockController = new LockController(this.server, this, this.sd.lockControllerData, DbKey.TakeLockControl(), DbKey.LockedHash(), DbKey.LockPrefix());

            this.AddServiceProxy(this.globalServiceProxy = new GlobalServiceProxy(this));
            this.taskQueueOwnersRedis = new TaskQueueOwnersRedis(this.server, DbKey.PersistenceTaskQueueOwners());

            #region auto_collection_var_create

            this.collection_user_info = new collection_user_info(server, this);
            this.collection_account_info = new collection_account_info(server, this);
            this.collection_scene_info = new collection_scene_info(server, this);
            this.collection_message_report_info = new collection_message_report_info(server, this);
            this.collection_user_report_info = new collection_user_report_info(server, this);
            this.collection_user_brief_info = new collection_user_brief_info(server, this);
            this.collection_private_room_info = new collection_private_room_info(server, this);

            #endregion auto_collection_var_create
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<DbService>();

            MongoRegister.Init();
        }
    }
}