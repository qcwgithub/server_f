namespace Script
{
    public static class RoomKey
    {
        public static string Location(long roomId) => "room:" + roomId + ":location";
    }
}