using Data;

namespace Script
{
    public class DbService : Service
    {
        public DbServiceData databaseServiceData
        {
            get
            {
                return (DbServiceData)this.data;
            }
        }

        public readonly ConnectToGlobalService connectToGlobalService;
        public readonly collection_user_profile collection_user_profile;

        public DbService(Server server, int serviceId) : base(server, serviceId)
        {
            this.AddConnectToOtherService(this.connectToGlobalService = new ConnectToGlobalService(this));
            this.collection_user_profile = new collection_user_profile().Init(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<DbService>();

            MongoRegister.Init();


            #region auto_collection_var_create

            #endregion auto_collection_var_create

            #region auto_handler_create

            #endregion auto_handler_create

            this.dispatcher.AddHandler(new Db_Start().Init(this.server, this));
            this.dispatcher.AddHandler(new Db_Shutdown().Init(this.server, this));

            this.dispatcher.AddHandler(new Db_InsertUserProfile().Init(this.server, this));
            this.dispatcher.AddHandler(new Db_QueryUserProfile().Init(this.server, this));
            this.dispatcher.AddHandler(new Db_SaveUserProfile().Init(this.server, this));

        }
    }
}