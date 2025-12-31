using Data;

namespace Script
{
    public class MessageDispatcher : ServiceScript<Service>
    {
        public MessageDispatcher(Server server, Service service) : base(server, service)
        {
        }

        Dictionary<MsgType, IHandler> handlers = new Dictionary<MsgType, IHandler>();

        ///////// handlers ///////////
        public IHandler? GetHandler(MsgType msgType)
        {
            return this.handlers.TryGetValue(msgType, out IHandler? handler) ? handler : null;
        }
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

        public List<MsgType> recentMsgTypes = new List<MsgType>();
        protected virtual ECode BeforeHandle(MessageContext context, MsgType type, object msg)
        {
            if (this.recentMsgTypes.Count > 10000)
            {
                // 防错而已
                this.recentMsgTypes.Clear();
            }
            this.recentMsgTypes.Add(type);

            return ECode.Success;
        }

        protected virtual void AfterHandle(MessageContext context, MsgType type, object msg, ECode e, object res)
        {
            if (e != ECode.Success && type.LogErrorIfNotSuccess())
            {
                // 已知 oldConnection 会走到这，不是错误
                this.service.logger.InfoFormat("{0} ECode.{1}", type, e);
            }
        }

        public virtual void OnFps(int fps)
        {
            if (fps < 5)
            {
                string s = JsonUtils.stringify(this.recentMsgTypes.GroupBy(e => e).ToDictionary(g => g.Key, g => g.Count()));
                this.service.logger.Warn($"fps {fps}, recent {s}");
            }

            this.recentMsgTypes.Clear();
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

        public async Task<MyResponse<Res>> Dispatch<Msg, Res>(MessageContext context, MsgType msgType, Msg msg)
            where Res : class
        {
            IHandler? handler = this.GetHandler(msgType);
            if (handler == null)
            {
                this.service.logger.ErrorFormat("no handler for message {0}", msgType);
                return new MyResponse<Res>(ECode.Error, null);
            }

            (ECode e, object res) = await this.DispatchImpl(context, handler, msgType, msg);
            return new MyResponse<Res>(e, (Res)res);
        }

        public async Task<(ECode, ArraySegment<byte>)> Dispatch(MessageContext context, MsgType msgType, ArraySegment<byte> msgData)
        {
            IHandler? handler = this.GetHandler(msgType);
            if (handler == null)
            {
                this.service.logger.ErrorFormat("no handler for message {0}", msgType);
                return (ECode.Error, []);
            }

            object msg = handler.DeserializeMsg(msgData);
            (ECode e, object res) = await this.DispatchImpl(context, handler, msgType, msg);

            ArraySegment<byte> resBytes = handler.SerializeRes(res);
            return (e, resBytes);
        }

        protected virtual void BeforePostHandle(MessageContext context, MsgType type, object msg, ECode e, object res)
        {

        }

        protected virtual void AfterPostHandle(MessageContext context, MsgType type, object msg, ECode e, object res)
        {

        }

        protected virtual async Task<(ECode, object)> DispatchImpl(MessageContext context, IHandler handler, MsgType type, object msg)
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

            ECode e = default;
            object res = default;

            int busyIndex = this.service.data.AddToBusyList((int)type);
            if (this.service.data.busyCount >= this.service.data.lastErrorBusyCount + 100)
            {
                this.service.data.lastErrorBusyCount = this.service.data.busyCount;
                this.service.logger.ErrorFormat("busyCount {0} detail: {1}", this.service.data.busyCount, this.FormatBusyList(this.service.data.busyList));
            }

            try
            {
                this.BeforeHandle(context, type, msg);
                (e, res) = await handler.Handle(context, msg);
                this.AfterHandle(context, type, msg, e, res);
            }
            catch (Exception ex)
            {
                this.service.logger.Fatal("disaptch exception 1! msgType: " + type, ex);
                e = ECode.Exception;
            }

            try
            {
                this.service.data.RemoveFromBusyList(busyIndex);
                this.BeforePostHandle(context, type, msg, e, res);
                (e, res) = handler.PostHandle(context, msg, e, res);
                this.AfterPostHandle(context, type, msg, e, res);
                return (e, res);
            }
            catch (Exception ex)
            {
                this.service.logger.Fatal("disaptch exception 2! msgType: " + type, ex);
                return (ECode.Exception, null);
            }
        }
    }
}