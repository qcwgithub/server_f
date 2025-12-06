using System.IO;
using System.Threading.Tasks;
using Data;
using System.Collections.Generic;

namespace Script
{
    public class OnViewMongoDumpList<S> : Handler<S, MsgViewMongoDumpList>
        where S : Service
    {
        public override MsgType msgType => MsgType._ViewMongoDumpList;

        public override Task<MyResponse> Handle(ProtocolClientData socket, MsgViewMongoDumpList msg)
        {
            var res = new ResViewMongoDumpList();
            res.directories = new List<string>();

            if (!Directory.Exists(msg.dir))
            {
                return new MyResponse(ECode.Success, res).ToTask();
            }

            string[] directories = Directory.GetDirectories(msg.dir);
            res.directories.AddRange(directories);
            return new MyResponse(ECode.Success, res).ToTask();
        }
    }
}