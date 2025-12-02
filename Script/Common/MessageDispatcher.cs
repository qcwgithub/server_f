using Data;

namespace Script
{
    public class MessageDispatcher : ServiceScript<Service>
    {
        Dictionary<MsgType, IHandler> handlers = new Dictionary<MsgType, IHandler>();

        ///////// handlers ///////////
        public void AddHandler(IHandler handler, bool isReplace = false)
        {
            // remove first
            if (isReplace)
            {
                bool removed = this.RemoveHandler(handler.msgType);
                MyDebug.Assert(removed);
            }
            this.handlers.Add(handler.msgType, handler);
        }
        public bool RemoveHandler(MsgType type)
        {
            return this.handlers.Remove(type);
        }
        private readonly Action<ECode, object> emptyReply = (e, r) => { };


        public List<MsgType> recentMsgTypes = new List<MsgType>();
        protected virtual void BeforeHandle(ProtocolClientData socket, MsgType type, object msg)
        {
            if (this.recentMsgTypes.Count > 10000)
            {
                // 防错而已
                this.recentMsgTypes.Clear();
            }
            this.recentMsgTypes.Add(type);
        }

        public virtual void OnFps(int fps)
        {
            if (fps < 5)
            {
                string s = JsonUtils.stringify(this.recentMsgTypes.GroupBy(e => e).ToDictionary(g => g.Key, g => g.Count()));

                if (this.service.data.current_resGetServiceConfigs != null && this.service.data.current_resGetServiceConfigs.open)
                {
                    this.service.logger.Fatal($"fps {fps}, recent {s}");
                }
                else
                {
                    this.service.logger.Warn($"fps {fps}, recent {s}");
                }
            }

            this.recentMsgTypes.Clear();
        }

        protected virtual void BeforePostHandle(ProtocolClientData socket, MsgType type, object msg, MyResponse r)
        {

        }

        string FormatBusyList(List<int> busyList)
        {
            var list = new List<(MsgType, int)>();
            for (int i = 0; i < busyList.Count; i++)
            {
                MsgType mt = (MsgType)busyList[i];
                if (list.Count == 0 || mt != list[list.Count - 1].Item1)
                {
                    list.Add((mt, 1));
                }
                else
                {
                    var last = list[list.Count - 1];
                    list[list.Count - 1] = (last.Item1, last.Item2 + 1);
                }
            }

            return string.Join(", ", list.Select(x => x.Item1.ToString() + "*" + x.Item2));
        }

        // about reply
        // 1 处理网络来的请求，reply 是回复请求
        // 2 自己调用 dispatch 的，reply 没什么用，为了统一，赋值为 utils.emptyReply
        // reply()的参数统一为 MyResponse
        public async void Dispatch(ProtocolClientData socket, MsgType type, object msg, Action<ECode, object> reply)
        {
            if (this.service.detached)
            {
                string message = string.Format("{0}.Disaptch MsgType.{1} server.detaching({2}) server.detached({3})",
                    this.GetType().Name, type, this.service.detaching, this.service.detached);

                Console.WriteLine(message);

                if (this.service.data != null)
                {
                    this.service.data.logger.Error(message);
                }
            }

            if (reply == null)
            {
                reply = this.emptyReply;
            }

            IHandler handler;
            if (!this.handlers.TryGetValue(type, out handler))
            {
                this.service.logger.ErrorFormat("no handler for message {0}", type);
                reply(ECode.Error, null);
                return;
            }

            MyResponse r = null;


            int busyIndex = this.service.data.AddToBusyList((int)type);
            if (this.service.data.busyCount >= this.service.data.lastErrorBusyCount + 100)
            {
                this.service.data.lastErrorBusyCount = this.service.data.busyCount;
                this.service.logger.ErrorFormat("busyCount {0} detail: {1}", this.service.data.busyCount, this.FormatBusyList(this.service.data.busyList));
            }
            try
            {
                if (socket != null && socket.msgProcessing != 0 && !type.CanParallel())
                {
                    // 消息处理中
                    r = new MyResponse(ECode.MsgProcessing, null);
                    this.service.logger.ErrorFormat("MsgType.{0} wait MsgType.{1}", type, (MsgType)socket.msgProcessing);
                }
                else
                {
                    if (socket != null && !type.CanParallel())
                    {
                        socket.msgProcessing = (int)type;
                    }

                    this.BeforeHandle(socket, type, msg);
                    r = await handler.Handle(socket, msg);

                    if (r.err != ECode.Success && type.LogErrorIfNotSuccess() && !r.HasDontLogErrorFlag())
                    {
                        if (socket.userId != 0)
                        {
                            this.service.logger.ErrorFormat("{0} playerId {1} ECode.{2}", type, socket.userId, r.err);
                        }
                        else
                        {
                            // 已知 oldSocket 会走到这，不是错误
                            this.service.logger.InfoFormat("{0} lastPlayerId {1} ECode.{2}", type, socket.lastUserId, r.err);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.service.logger.Fatal("disaptch exception 1! msgType: " + type, ex);
                r = new MyResponse(ECode.Exception, null);
            }

            try
            {
                if (socket != null)
                {
                    socket.msgProcessing = 0;
                }
                this.service.data.RemoveFromBusyList(busyIndex);
                this.BeforePostHandle(socket, type, msg, r);
                r = handler.PostHandle(socket, msg, r);
                reply(r.err, r.res == null ? null : r.res);
            }
            catch (Exception ex)
            {
                this.service.logger.Fatal("disaptch exception 2! msgType: " + type, ex);
                reply(ECode.Exception, null);
            }
        }
    }
}