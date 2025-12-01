using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using System.Diagnostics;

namespace Data
{
    public partial class BinaryMessagePacker : IMessagePacker
    {
        private static BinaryMessagePacker instance;
        public static BinaryMessagePacker Instance
        {
            get
            {
                if (instance == null) instance = new BinaryMessagePacker();
                return instance;
            }
        }

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
            return 3 * sizeof(int) + 3;
        }
        public void ModifySeq(byte[] buffer, int seq)
        {
            int offset = sizeof(int);
            this.WriteInt(buffer, offset, seq);
        }
        void PackHeader(byte[] buffer, int seq, int msgTypeOrECode, byte msgCount, bool isList, bool requireResponse, ref int bufferOffset)
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

            // 1 = msgCount
            buffer[bufferOffset] = msgCount;
            bufferOffset++;

            // 1 = isList
            buffer[bufferOffset] = (isList ? (byte)1 : (byte)0);
            bufferOffset++;
        }

        public byte[] Pack(int msgTypeOrECode, object msg, int seq, bool requireResponse)
        {
            byte[] buffer = null;
            int bufferOffset = 0;
            int totalLength = this.GetHeaderLength();
            if (msg is List<object> list)
            {
                int L = list.Count;
                byte msgCount = (byte)L;

                var codeList = new List<MessageCode>();
                var bytesList = new List<byte[]>();
                for (int i = 0; i < L; i++)
                {
                    object obj = list[i];

                    // a
                    MessageCode messageCode = TypeToMessageCodeCache.getMessageCode(obj);
                    codeList.Add(messageCode);
                    totalLength += sizeof(short);

                    // b
                    totalLength += sizeof(int);

                    // c
                    byte[] subBytes = PackBody(messageCode, obj);
                    bytesList.Add(subBytes);
                    totalLength += subBytes.Length;
                }

                buffer = new byte[totalLength];
                this.PackHeader(buffer, seq, msgTypeOrECode, msgCount, true, requireResponse, ref bufferOffset);

                for (int i = 0; i < L; i++)
                {
                    // a
                    this.WriteShort(buffer, bufferOffset, (short)codeList[i]);
                    bufferOffset += sizeof(short);

                    // b
                    this.WriteInt(buffer, bufferOffset, bytesList[i].Length);
                    bufferOffset += sizeof(int);

                    // c
                    Array.Copy(bytesList[i], 0, buffer, bufferOffset, bytesList[i].Length);
                    bufferOffset += bytesList[i].Length;
                }
            }
            else
            {
                // a
                MessageCode messageCode = TypeToMessageCodeCache.getMessageCode(msg);
                totalLength += sizeof(short);

                // b
                totalLength += sizeof(int);

                // c
                byte[] bytes = PackBody(messageCode, msg);
                totalLength += bytes.Length;

                buffer = new byte[totalLength];
                this.PackHeader(buffer, seq, msgTypeOrECode, msgCount: 1, false, requireResponse, ref bufferOffset);

                // a
                this.WriteShort(buffer, bufferOffset, (short)messageCode);
                bufferOffset += sizeof(short);

                // b
                this.WriteInt(buffer, bufferOffset, bytes.Length);
                bufferOffset += sizeof(int);

                // c
                Array.Copy(bytes, 0, buffer, bufferOffset, bytes.Length);
                bufferOffset += bytes.Length;
            }

            MyDebug.Assert(bufferOffset == buffer.Length);
            return buffer;
        }

        void UnpackHeader(ref UnpackResult r, byte[] buffer, ref int offset, ref int count, out byte msgCount, out bool isList)
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

            // 1 = msgCount
            msgCount = buffer[offset];
            offset++;
            count--;

            isList = buffer[offset] == 1;
            offset++;
            count--;
        }

        public UnpackResult Unpack(byte[] buffer, int offset, int count)
        {
            var r = new UnpackResult();

            byte msgCount;
            bool isList;
            this.UnpackHeader(ref r, buffer, ref offset, ref count, out msgCount, out isList);

            List<object> list = null;
            if (isList)
            {
                r.msg = list = new List<object>();
            }

            for (int i = 0; i < msgCount; i++)
            {
                // a
                MessageCode messageCode = (MessageCode)this.ReadShort(buffer, offset);
                offset += sizeof(short);
                count -= sizeof(short);

                // b
                int length = this.ReadInt(buffer, offset);
                offset += sizeof(int);
                count -= sizeof(int);

                // c
                object msg = UnpackBody(messageCode, buffer, offset, length);
                offset += length;
                count -= length;

                if (!isList)
                {
                    r.msg = msg;
                }
                else
                {
                    list.Add(msg);
                }
            }

            MyDebug.Assert(count == 0);

            r.success = true;
            return r;
        }
    }
}