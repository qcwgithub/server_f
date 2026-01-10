namespace Data
{
    public struct UnpackResult
    {
        public bool success;

        public int totalLength;

        public int seq;
        public int code;

        // 1 of 2
        public int typeLength; // ---

        // 对方是否要求回复
        public bool requireResponse;

        public byte[] msgBytes;
    }

    public interface IMessagePacker
    {
        bool IsCompeteMessage(byte[] buffer, int offset, int count, out int exactCount);
        UnpackResult Unpack(byte[] buffer, int offset, int count);
        byte[] Pack(int msgTypeOrECode, byte[] msg, int seq, bool requireResponse);
        void ModifySeq(byte[] buffer, int seq);
    }
}