using System.IO;
using System.Threading.Tasks;
using Data;
using System.Collections.Generic;

namespace Script
{
    public class OnViewMongoDumpList<S> : Handler<S, MsgViewMongoDumpList, ResViewMongoDumpList>
        where S : Service
    {
        public OnViewMongoDumpList(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._ViewMongoDumpList;

        public override async Task<ECode> Handle(MsgContext context, MsgViewMongoDumpList msg, ResViewMongoDumpList res)
        {
            res.directories = new List<string>();

            if (!Directory.Exists(msg.dir))
            {
                return ECode.Success;
            }

            string[] directories = Directory.GetDirectories(msg.dir);
            res.directories.AddRange(directories);
            return ECode.Success;
        }
    }
}