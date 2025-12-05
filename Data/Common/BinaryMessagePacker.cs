namespace Data
{
    public partial class BinaryMessagePacker : IMessagePacker
    {
        public bool IsCompeteMessage(byte[] buffer, int offset, int count, out int exactCount)
        {
            exactCount = 0;

            if (count < GetHeaderLength())
            {
                return false;
            }

            int length = this.ReadInt(buffer, offset);
            if (count < length)
            {
                return false;
            }

            exactCount = length;
            return true;
        }

        protected void WriteInt(byte[] buffer, int offset, int value_)
        {
            unchecked
            {
                uint value = (uint)value_;
                buffer[offset + 0] = (byte)(value);
                buffer[offset + 1] = (byte)(value >> 8);
                buffer[offset + 2] = (byte)(value >> 16);
                buffer[offset + 3] = (byte)(value >> 24);
            }
        }
        protected int ReadInt(byte[] buffer, int offset)
        {
            unchecked
            {
                uint value = 0;
                value += buffer[offset];
                value += (((uint)buffer[offset + 1]) << 8);
                value += (((uint)buffer[offset + 2]) << 16);
                value += (((uint)buffer[offset + 3]) << 24);
                return (int)value;
            }
        }
        protected void WriteShort(byte[] buffer, int offset, short value)
        {
            unchecked
            {
                buffer[offset + 0] = (byte)(value << 8 >> 8);
                buffer[offset + 1] = (byte)(value << 0 >> 8);
            }
        }
        protected short ReadShort(byte[] buffer, int offset)
        {
            unchecked
            {
                ushort value = 0;
                value += buffer[offset];
                value += (ushort)(((ushort)buffer[offset + 1]) << 8);
                return (short)value;
            }
        }

        int GetHeaderLength()
        {
            return 3 * sizeof(int) + 1;
        }

        public void ModifySeq(byte[] buffer, int seq)
        {
            int offset = sizeof(int);
            this.WriteInt(buffer, offset, seq);
        }

        void PackHeader(byte[] buffer, int seq, int msgTypeOrECode, bool requireResponse, ref int bufferOffset)
        {
            // 4 = totalLength
            this.WriteInt(buffer, bufferOffset, buffer.Length);
            bufferOffset += sizeof(int);

            // 4 = seq
            this.WriteInt(buffer, bufferOffset, seq);
            bufferOffset += sizeof(int);

            // 4 = ECode/MsgType
            this.WriteInt(buffer, bufferOffset, msgTypeOrECode);
            bufferOffset += sizeof(int);

            // 1 = require response?
            buffer[bufferOffset] = (requireResponse ? (byte)1 : (byte)0);
            bufferOffset++;
        }

        public byte[] Pack(int msgTypeOrECode, byte[] msg, int seq, bool requireResponse)
        {
            byte[] buffer;
            int bufferOffset = 0;
            int totalLength = this.GetHeaderLength();

            // b
            totalLength += sizeof(int);

            // c
            totalLength += msg.Length;

            buffer = new byte[totalLength];
            this.PackHeader(buffer, seq, msgTypeOrECode, requireResponse, ref bufferOffset);

            // b
            this.WriteInt(buffer, bufferOffset, msg.Length);
            bufferOffset += sizeof(int);

            // c
            Array.Copy(msg, 0, buffer, bufferOffset, msg.Length);
            bufferOffset += msg.Length;

            MyDebug.Assert(bufferOffset == buffer.Length);
            return buffer;
        }

        void UnpackHeader(ref UnpackResult r, byte[] buffer, ref int offset, ref int count)
        {
            // 4 = total length
            r.totalLength = this.ReadInt(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 4 = seq
            r.seq = this.ReadInt(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 4 = ECode/MsgType
            r.code = this.ReadInt(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 1 = require response
            r.requireResponse = buffer[offset] == 1;
            offset++;
            count--;
        }

        public UnpackResult Unpack(byte[] buffer, int offset, int count)
        {
            var r = new UnpackResult();
            this.UnpackHeader(ref r, buffer, ref offset, ref count);

            // b
            int length = this.ReadInt(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // c
            r.msg = new ArraySegment<byte>(buffer, offset, length);
            offset += length;
            count -= length;

            MyDebug.Assert(count == 0);

            r.success = true;
            return r;
        }
    }
}