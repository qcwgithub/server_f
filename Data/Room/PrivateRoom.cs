namespace Data
{
    public class PrivateRoom : Room
    {
        public readonly PrivateRoomInfo privateRoomInfo;
        public PrivateRoom(PrivateRoomInfo privateRoomInfo)
        {
            this.roomType = RoomType.Private;
            this.roomId = privateRoomInfo.roomId;
            this.privateRoomInfo = privateRoomInfo;
        }

        public PrivateRoomInfo? lastPrivateRoomInfo;
        public override void OnAddedToDict()
        {
            // 有值就不能再赋值了，不然玩家上线下线就错了
            MyDebug.Assert(this.lastPrivateRoomInfo == null);

            this.lastPrivateRoomInfo = PrivateRoomInfo.Ensure(null);
            this.lastPrivateRoomInfo.DeepCopyFrom(this.privateRoomInfo);
        }
    }
}