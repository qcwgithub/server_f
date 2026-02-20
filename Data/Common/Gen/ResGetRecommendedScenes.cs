using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResGetRecommendedScenes
    {
        [Key(0)]
        public List<SceneInfo> sceneInfos;
    }
}
