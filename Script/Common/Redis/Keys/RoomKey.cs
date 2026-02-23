namespace Script
{
    public static class RoomKey
    {
        public static string Location(long roomId) => "room:" + roomId + ":location";
        public static string Messages(long roomId) => "room:" + roomId + ":messages";
        public static string MessagesDirty(long roomId) => "room:" + roomId + ":messagesDirty";
    }
}