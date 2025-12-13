using Data;

namespace Script
{
    public class User_Start : OnStart<UserService>
    {
        public User_Start(Server server, UserService service) : base(server, service)
        {
        }

        protected override async Task<ECode> Handle2()
        {
            return ECode.Success;
        }
    }
}