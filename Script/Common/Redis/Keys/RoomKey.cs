namespace Script
{
    public static class RoomKey
    {
        public static string Location(long roomId) => "room:" + roomId + ":location";
        public static string SceneMessages(long roomId) => "sceneRoom:" + roomId + ":messages";
        public static string FriendChatMessages(long roomId) => "friendChatRoom:" + roomId + ":messages";
    }
}