using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResSearchScene
    {
        [Key(0)]
        public List<SceneRoomInfo> sceneRoomInfos;
    }
}
