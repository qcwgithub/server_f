using System.Collections.Generic;
using System;

namespace Data
{
    public interface IServer
    {
        void FrameStart(long frame);
        void FrameEnd(long frame);
        void Attach(Dictionary<string, string> args, ServerData serverData, int seq);
        int GetScriptDllSeq();
        Version GetScriptDllVersion();
        void Detach();
        void HandleEvent(string event_);
    }
}