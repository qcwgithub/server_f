using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnGetPendingMsgList<S> : Handler<S>
        where S : Service
    {
        public override MsgType msgType => MsgType._GetPendingMessageList;

        public override Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgGetPendingMsgList>(_msg);
            var res = new ResGetPendingMsgList();
            res.list = new List<int>();
            res.list.AddRange(this.service.data.busyList);
            return new MyResponse(ECode.Success, res).ToTask();
        }
    }
}