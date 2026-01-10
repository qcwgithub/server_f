namespace Data
{
    // Called by main thread
    public interface IProtocolClient
    {
        bool IsClosed();
        void Connect();
        void Send(byte[] bytes);
        void Close(string reason);
        string? closeReason { get; }
    }
}