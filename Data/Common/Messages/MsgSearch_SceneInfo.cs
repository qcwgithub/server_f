using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSearch_SceneInfo
    {
        [Key(0)]
        public string keyword;
    }

    [MessagePackObject]
    public class ResSearch_SceneInfo
    {
        [Key(0)]
        public List<SceneInfo> sceneInfos;
    }
}