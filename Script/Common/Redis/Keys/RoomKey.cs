namespace Script
{
    public static class RoomKey
    {
        public static string OwningServiceId(long roomId) => "room:" + roomId + ":owningServiceId";
    }
}