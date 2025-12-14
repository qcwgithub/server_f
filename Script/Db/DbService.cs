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
        
        #region auto_collection_var_decl

        public collection_user_info collection_user_info;
        public collection_account_info collection_account_info;

        #endregion auto_collection_var_decl

        public DbService(Server server, int serviceId) : base(server, serviceId)
        {
            this.lockController = new LockController(this.server, this, this.sd.lockControllerData, DbKey.TakeLockControl(), DbKey.LockedHash(), DbKey.LockPrefix());

            this.AddConnectToOtherService(this.connectToGlobalService = new ConnectToGlobalService(this));

            #region auto_collection_var_create

            this.collection_user_info = new collection_user_info(server, this);
            this.collection_account_info = new collection_account_info(server, this);

            #endregion auto_collection_var_create
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<DbService>();

            MongoRegister.Init();

            #region auto_handler_create

            this.dispatcher.AddHandler(new Query_UserInfo_by_userId(server, this));
            this.dispatcher.AddHandler(new Save_AccountInfo(server, this));
            this.dispatcher.AddHandler(new Query_AccountInfo_by_channel_channelUserId(server, this));
            this.dispatcher.AddHandler(new Query_AccountInfo_by_channelUserId(server, this));
            this.dispatcher.AddHandler(new Query_AccountInfo_byElementOf_userIds(server, this));
            this.dispatcher.AddHandler(new Query_listOf_AccountInfo_byElementOf_userIds(server, this));

            #endregion auto_handler_create

            this.dispatcher.AddHandler(new Db_Start(this.server, this));
            this.dispatcher.AddHandler(new Db_Shutdown(this.server, this));

            this.dispatcher.AddHandler(new Db_InsertUserInfo(this.server, this));
            this.dispatcher.AddHandler(new Save_UserInfo(this.server, this));
        }
    }
}