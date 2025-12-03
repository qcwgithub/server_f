using Data;

namespace Script
{
    public class DatabaseService : Service
    {
        public DatabaseServiceData databaseServiceData
        {
            get
            {
                return (DatabaseServiceData)this.data;
            }
        }

        public DatabaseService(Server server, int serviceId) : base(server, serviceId)
        {
        }

        public collection_user collection_user;
        public ConnectToGlobalService connectToGlobalService { get; private set; }

        public override void Attach()
        {
            base.Attach();

            base.AddHandler<DatabaseService>();
            this.AddConnectToOtherService(this.connectToGlobalService = new ConnectToGlobalService(this));

            MongoRegister.Init();


            #region auto_collection_var_create

            #endregion auto_collection_var_create

            #region auto_handler_create

            #endregion auto_handler_create
            this.collection_user = new collection_user().Init(this.server, this);

            this.dispatcher.AddHandler(new Database_Start().Init(this.server, this));
            this.dispatcher.AddHandler(new Database_Shutdown().Init(this.server, this));

            this.dispatcher.AddHandler(new Database_InsertUser().Init(this.server, this));

            this.dispatcher.AddHandler(new Database_QueryUser_byId().Init(this.server, this));
            this.dispatcher.AddHandler(new Database_SaveUser().Init(this.server, this));
        }
    }
}