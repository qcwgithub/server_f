using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class UserInfo_Db : IIsDifferent_Db<UserInfo>
    {
        [BsonIgnoreIfNull]
        public long? userId;
        [BsonIgnoreIfNull]
        public string userName;
        [BsonIgnoreIfNull]
        public long? createTimeS;
        [BsonIgnoreIfNull]
        public long? lastLoginTimeS;
        [BsonIgnoreIfNull]
        public long? lastSetNameTimeS;
        [BsonIgnoreIfNull]
        public int? avatarIndex;
        [BsonIgnoreIfNull]
        public long? lastSetAvatarIndexTimeS;
        [BsonIgnoreIfNull]
        public List<FriendInfo_Db> friends;
        [BsonIgnoreIfNull]
        public List<OutgoingFriendRequest_Db> outgoingFriendRequests;
        [BsonIgnoreIfNull]
        public List<IncomingFriendRequest_Db> incomingFriendRequests;
        [BsonIgnoreIfNull]
        public List<BlockedUser_Db> blockedUsers;

        public bool DeepCopyFrom(UserInfo other)
        {
            bool empty = true;

            this.userId = XInfoHelper_Db.Copy_long(other.userId);
            if (this.userId != null)
            {
                empty = false;
            }

            this.userName = XInfoHelper_Db.Copy_string(other.userName);
            if (this.userName != null)
            {
                empty = false;
            }

            this.createTimeS = XInfoHelper_Db.Copy_long(other.createTimeS);
            if (this.createTimeS != null)
            {
                empty = false;
            }

            this.lastLoginTimeS = XInfoHelper_Db.Copy_long(other.lastLoginTimeS);
            if (this.lastLoginTimeS != null)
            {
                empty = false;
            }

            this.lastSetNameTimeS = XInfoHelper_Db.Copy_long(other.lastSetNameTimeS);
            if (this.lastSetNameTimeS != null)
            {
                empty = false;
            }

            this.avatarIndex = XInfoHelper_Db.Copy_int(other.avatarIndex);
            if (this.avatarIndex != null)
            {
                empty = false;
            }

            this.lastSetAvatarIndexTimeS = XInfoHelper_Db.Copy_long(other.lastSetAvatarIndexTimeS);
            if (this.lastSetAvatarIndexTimeS != null)
            {
                empty = false;
            }

            this.friends = XInfoHelper_Db.Copy_ListClass<FriendInfo_Db, FriendInfo>(other.friends);
            if (this.friends != null)
            {
                empty = false;
            }

            this.outgoingFriendRequests = XInfoHelper_Db.Copy_ListClass<OutgoingFriendRequest_Db, OutgoingFriendRequest>(other.outgoingFriendRequests);
            if (this.outgoingFriendRequests != null)
            {
                empty = false;
            }

            this.incomingFriendRequests = XInfoHelper_Db.Copy_ListClass<IncomingFriendRequest_Db, IncomingFriendRequest>(other.incomingFriendRequests);
            if (this.incomingFriendRequests != null)
            {
                empty = false;
            }

            this.blockedUsers = XInfoHelper_Db.Copy_ListClass<BlockedUser_Db, BlockedUser>(other.blockedUsers);
            if (this.blockedUsers != null)
            {
                empty = false;
            }

            return !empty;
        }
    }
}
