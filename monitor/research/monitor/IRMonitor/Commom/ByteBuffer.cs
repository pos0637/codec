using System;

namespace Common
{
    public class ByteBuffer
    {
        private const Int32 DEFAULT_BUFFER_LENGTH = 600 * 1024;
        private const Int32 PENDING_BUFFER_LENGTH = 1;

        public Int32 mCapacity;
        public Int32 mUsed;
        public Byte[] mBuffer;

        public ByteBuffer()
        {
            mBuffer = new Byte[DEFAULT_BUFFER_LENGTH];
            mCapacity = DEFAULT_BUFFER_LENGTH;
            mUsed = 0;
        }

        public ByteBuffer(Int32 capacity)
        {
            mBuffer = new Byte[capacity];
            mCapacity = capacity;
            mUsed = 0;
        }

        public void Clear()
        {
            mUsed = 0;
        }

        public Int32 Push(Byte[] data, Int32 used)
        {
            if (used <= 0)
                return 0;

            // resize buffer
            if ((mUsed + used) > mCapacity) {
                Int32 capacity = mUsed + used + PENDING_BUFFER_LENGTH;
                try {
                    Array.Resize(ref mBuffer, capacity);
                    mCapacity = capacity;
                }
                catch {
                    return -1;
                }
            }

            // append
            Array.Copy(data, 0, mBuffer, mUsed, used);
            mUsed += used;

            return 0;
        }

        public unsafe Int32 Strip(Int32 length)
        {
            if ((length == 0) || (length > mUsed))
                return -1;

            fixed (Byte* data = mBuffer) {
                Byte* src = data + length;
                Byte* dst = data;
                mUsed -= length;
                Int32 used = mUsed;

                while ((used--) > 0) {
                    *dst++ = *src++;
                }
            }

            return 0;
        }

        public Boolean ReadByteArray(Int32 position, Byte[] buffer, Int32 count)
        {
            if (count + position > mUsed)
                return false;
            else {
                Array.Copy(mBuffer, position, buffer, 0, count);
                return true;
            }
        }

        public Byte ReadByte(Int32 position)
        {
            return mBuffer[position];
        }

        public Int32 ReadInt32(Int32 position)
        {
            return (mBuffer[position + 3] << 24)
                | (mBuffer[position + 2] << 16)
                | (mBuffer[position + 1] << 8)
                | (mBuffer[position]);
        }
    }
}
