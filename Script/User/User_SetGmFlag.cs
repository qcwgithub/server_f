
// 运维，GM功能
using Data;

namespace Script
{
    public class User_SetGmFlag : UserHandler
    {
        public override MsgType msgType => MsgType._SetGmFlag;

        public override Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSetGmFlag>(_msg);

            var res = new ResSetGmFlag();
            res.listUser = new List<long>();
            for (long i = msg.startUserId; i <= msg.endUserId; i++)
            {
                User? user = this.service.sd.GetUser(i);
                if (user != null)
                {
                    user.isGm = true;
                    res.listUser.Add(i);
                }
            }
            
            return new MyResponse(ECode.Success, res).ToTask();
        }
    }
}