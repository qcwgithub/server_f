using Data;

namespace Script
{
    [AutoRegister]
    public class User_ReceiveFriendChatMessages : Handler<UserService, MsgReceiveFriendChatMessages, ResReceiveFriendChatMessages>
    {
        public override MsgType msgType => MsgType.ReceiveFriendChatMessages;
        public User_ReceiveFriendChatMessages(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgReceiveFriendChatMessages msg, ResReceiveFriendChatMessages res)
        {
            string log = $"{this.msgType} userId {context.msg_userId}";
            this.service.logger.Info(log);

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            res.messageListDict = new();
            res.hasMore = false;

            UserInfo userInfo = user.userInfo;
            if (userInfo.friends.Count == 0)
            {
                return ECode.Success;
            }

            var roomIds = userInfo.friends.Select(x => x.roomId).ToList();
            var tasks = roomIds.Select(x => this.server.friendChatMessagesRedis.GetAll(x)).ToArray();

            var msgDb = new MsgQuery_FriendChatMessages_by_roomId_receivedSeqs();
            msgDb.roomIdToReceivedSeqs = new Dictionary<long, long>();
            foreach (FriendInfo friendInfo in userInfo.friends)
            {
                msgDb.roomIdToReceivedSeqs[friendInfo.roomId] = friendInfo.receivedSeq;
            }

            var taskMongo = this.service.dbServiceProxy.Query_FriendChatMessages_by_roomId_receivedSeqs(msgDb);
            await Task.WhenAll(tasks.Cast<Task>().Append(taskMongo));

            var r = taskMongo.Result;
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var resDb = r.CastRes<ResQuery_FriendChatMessages_by_roomId_receivedSeqs>();

            var dictDict = new Dictionary<long, Dictionary<long, ChatMessage>>();

            int totalMongo = 0;

            // 先将 MongoDB 数据写入
            foreach (ChatMessage message in resDb.messages)
            {
                totalMongo++;

                long roomId = message.roomId;
                if (!msgDb.roomIdToReceivedSeqs.TryGetValue(roomId, out long receivedSeq))
                {
                    this.service.logger.Error($"{log} MongoDB !msgDb.roomIdToReceivedSeqs.TryGetValue(roomId {roomId}, ...)");
                    continue;
                }

                if (message.seq <= receivedSeq)
                {
                    this.service.logger.Error($"{log} MongoDB message.seq {message.seq} <= receivedSeq {receivedSeq}");
                    continue;
                }

                if (!dictDict.TryGetValue(roomId, out Dictionary<long, ChatMessage>? messageDict))
                {
                    messageDict = new Dictionary<long, ChatMessage>();
                    dictDict[roomId] = messageDict;
                }

                messageDict[message.seq] = message;
            }

            if (totalMongo > 10000)
            {
                this.service.logger.Error($"{log} MongoDB total {totalMongo} messages");
            }

            int totalRedis = 0;
            bool logRedisSeqError = true;

            // 再将 Redis 数据覆盖
            for (int i = 0; i < tasks.Length; i++)
            {
                long roomId = roomIds[i];
                long receivedSeq = msgDb.roomIdToReceivedSeqs[roomId];
                ChatMessage[] messages = tasks[i].Result;
                totalRedis += messages.Length;

                if (!dictDict.TryGetValue(roomId, out Dictionary<long, ChatMessage>? messageDict))
                {
                    messageDict = new Dictionary<long, ChatMessage>();
                    dictDict[roomId] = messageDict;
                }

                for (int j = 0; j < messages.Length; j++)
                {
                    ChatMessage message = messages[j];
                    if (j > 0 && message.seq <= messages[j - 1].seq)
                    {
                        if (logRedisSeqError)
                        {
                            logRedisSeqError = false;
                            this.service.logger.Error($"{log} Redis roomId {roomId} message.seq {message.seq} <= previousSeq {messages[j - 1].seq}(Log once only)");
                        }
                    }
                    if (message.seq <= receivedSeq)
                    {
                        // Not error, just skip
                        continue;
                    }

                    messageDict[message.seq] = message;
                }
            }

            if (totalRedis > 10000)
            {
                this.service.logger.Error($"{log} Redis total {totalRedis} messages");
            }

            foreach (var kv in dictDict)
            {
                long roomId = kv.Key;
                List<ChatMessage> messageList = kv.Value.Values.OrderBy(x => x.seq).ToList();
                res.messageListDict[roomId] = new ChatMessageList { list = messageList };
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgReceiveFriendChatMessages msg, ECode e, ResReceiveFriendChatMessages res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}