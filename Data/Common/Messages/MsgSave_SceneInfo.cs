using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSave_SceneInfo
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        required public SceneInfoNullable sceneInfoNullable;
        [Key(2)]
        public SceneInfo? sceneInfo_debug;
    }

    [MessagePackObject]
    public class ResSave_SceneInfo
    {
    }
}