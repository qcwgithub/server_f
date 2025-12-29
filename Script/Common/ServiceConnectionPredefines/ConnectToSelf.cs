using Data;

namespace Script
{
    public class ConnectToSelf
    {
        Service self;
        public ConnectToSelf(Service self)
        {
            this.self = self;
        }

        public async Task<MyResponse<Res>> Request<Msg, Res>(MsgType msgType, Msg msg)
            where Res : class
        {
            return await this.self.dispatcher.Dispatch<Msg, Res>(default, msgType, msg);
        }
    }
}