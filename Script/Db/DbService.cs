using Data;

namespace Script
{
    public class DbService : Service
    {
        public DbServiceData sd
        {
            get
            {
                return (DbServiceData)this.data;
            }
        }

        public readonly LockController lockController;
        public readonly ConnectToGlobalService connectToGlobalService;
        public readonly collection_user_info collection_user_info;

        public DbService(Server server, int serviceId) : base(server, serviceId)
        {
            this.lockController = new LockController(this.server, this, this.sd.lockControllerData, DbKey.TakeLockControl(), DbKey.LockedHash(), DbKey.LockPrefix());

            this.AddConnectToOtherService(this.connectToGlobalService = new ConnectToGlobalService(this));
            this.collection_user_info = new collection_user_info(this.server, this);
        }

        #region auto_collection_var_decl

        public collection_account_info collection_account_info;

        #endregion auto_collection_var_decl

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<DbService>();

            MongoRegister.Init();


            #region auto_collection_var_create

            this.collection_account_info = new collection_account_info(server, this);

            #endregion auto_collection_var_create

            #region auto_handler_create

            this.dispatcher.AddHandler(new Save_AccountInfo(server, this));
            this.dispatcher.AddHandler(new Query_AccountInfo_by_channel_channelUserId(server, this));
            this.dispatcher.AddHandler(new Query_AccountInfo_by_channelUserId(server, this));
            this.dispatcher.AddHandler(new Query_AccountInfo_byElementOf_userIds(server, this));
            this.dispatcher.AddHandler(new Query_listOf_AccountInfo_byElementOf_userIds(server, this));

            #endregion auto_handler_create

            this.dispatcher.AddHandler(new Db_Start(this.server, this));
            this.dispatcher.AddHandler(new Db_Shutdown(this.server, this));

            this.dispatcher.AddHandler(new Db_InsertUserInfo(this.server, this));
            this.dispatcher.AddHandler(new Db_QueryUserProfile(this.server, this));
            this.dispatcher.AddHandler(new Db_SaveUserProfile(this.server, this));
        }
    }
}