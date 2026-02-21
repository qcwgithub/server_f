namespace Data
{
    public sealed class RoomServiceData : ServiceData
    {
        public readonly Dictionary<long, Room> roomDict;
        public int roomCount
        {
            get
            {
                return this.roomDict.Count;
            }
        }
        public T? GetRoomAs<T>(long roomId) where T : Room
        {
            return this.roomDict.TryGetValue(roomId, out Room? room) ? room as T : null;
        }
        public Room? GetRoom(long roomId)
        {
            return this.roomDict.TryGetValue(roomId, out Room? room) ? room : null;
        }
        public bool RemoveRoom(long roomId)
        {
            if (this.roomDict.Remove(roomId))
            {
                this.roomCountDelta--;
                return true;
            }
            return false;
        }
        public void AddRoom(Room room)
        {
            this.roomCountDelta++;
            this.roomDict.Add(room.roomId, room);
        }

        public int roomCountDelta = 0;
        public int destroyTimeoutS = 600;
        public int saveIntervalS = 60;
        public bool allowNewRoom;

        public readonly ObjectLocatorData userLocatorData;
        //------------------------------------------------------

        public static readonly List<ServiceType> s_connectToServiceIds = new List<ServiceType>
        {
            ServiceType.Db,
            ServiceType.Global,
            ServiceType.RoomManager,
        };
        public RoomServiceData(ServerData serverData, ServiceTypeAndId serviceTypeAndId)
            : base(serverData, serviceTypeAndId, s_connectToServiceIds)
        {
            this.roomDict = new Dictionary<long, Room>();

            this.LoadConfigs();

            this.allowNewRoom = true;
            this.userLocatorData = new ObjectLocatorData();
        }

        void LoadConfigs()
        {

        }

        public override void ReloadConfigs(bool all, List<string> files)
        {
            if (all)
            {
                this.LoadConfigs();
            }
            else
            {

            }
        }

        public override void GetReloadConfigOptions(List<string> files)
        {
            files.Add("all");
        }

        public ITimer timer_tick_loop;

        public class LockedRoom
        {
            public object? owner;
            public List<TaskCompletionSource>? waiting;
        }
        public readonly Dictionary<long, LockedRoom> lockedRoomDict = new();
        public bool IsRoomLocked(long roomId)
        {
            return this.lockedRoomDict.TryGetValue(roomId, out var lockedRoom) && lockedRoom.owner != null;
        }
    }
}