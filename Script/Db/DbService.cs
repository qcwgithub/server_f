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
        public collection_room_info collection_room_info;
        public collection_room_message_report_info collection_room_message_report_info;
        public collection_user_report_info collection_user_report_info;

        #endregion auto_collection_var_decl

        public DbService(Server server, int serviceId) : base(server, serviceId)
        {
            this.lockController = new LockController(this.server, this, this.sd.lockControllerData, DbKey.TakeLockControl(), DbKey.LockedHash(), DbKey.LockPrefix());

            this.AddServiceProxy(this.globalServiceProxy = new GlobalServiceProxy(this));
            this.taskQueueOwnersRedis = new TaskQueueOwnersRedis(this.server, DbKey.PersistenceTaskQueueOwners());

            #region auto_collection_var_create

            this.collection_user_info = new collection_user_info(server, this);
            this.collection_account_info = new collection_account_info(server, this);
            this.collection_room_info = new collection_room_info(server, this);
            this.collection_room_message_report_info = new collection_room_message_report_info(server, this);
            this.collection_user_report_info = new collection_user_report_info(server, this);

            #endregion auto_collection_var_create
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<DbService>();

            MongoRegister.Init();

            #region auto_handler_create

            this.dispatcher.AddHandler(new Query_UserInfo_by_userId(server, this));
            this.dispatcher.AddHandler(new Query_UserInfo_maxOf_userId(server, this));
            this.dispatcher.AddHandler(new Save_AccountInfo(server, this));
            this.dispatcher.AddHandler(new Query_AccountInfo_by_channel_channelUserId(server, this));
            this.dispatcher.AddHandler(new Query_AccountInfo_by_channelUserId(server, this));
            this.dispatcher.AddHandler(new Query_AccountInfo_byElementOf_userIds(server, this));
            this.dispatcher.AddHandler(new Query_listOf_AccountInfo_byElementOf_userIds(server, this));
            this.dispatcher.AddHandler(new Query_RoomInfo_by_roomId(server, this));
            this.dispatcher.AddHandler(new Query_RoomInfo_maxOf_roomId(server, this));
            this.dispatcher.AddHandler(new Save_RoomMessageReportInfo(server, this));
            this.dispatcher.AddHandler(new Save_UserReportInfo(server, this));

            #endregion auto_handler_create

            this.dispatcher.AddHandler(new Insert_UserInfo(this.server, this));
            this.dispatcher.AddHandler(new Save_UserInfo(this.server, this));

            this.dispatcher.AddHandler(new Insert_RoomInfo(this.server, this));
            this.dispatcher.AddHandler(new Save_RoomInfo(this.server, this));
            this.dispatcher.AddHandler(new Search_RoomInfo(this.server, this));

            this.dispatcher.AddHandler(new Db_OnTimer(this.server, this), true);
        }
    }
}