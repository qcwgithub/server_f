using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgWaitTask
    {
        public Task task;
    }

    [MessagePackObject]
    public class ResWaitTask
    {

    }
}