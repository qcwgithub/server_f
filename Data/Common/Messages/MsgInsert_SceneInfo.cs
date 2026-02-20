using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgInsert_SceneInfo
    {
        [Key(0)]
        public SceneInfo sceneInfo;
    }

    [MessagePackObject]
    public class ResInsert_SceneInfo
    {

    }
}