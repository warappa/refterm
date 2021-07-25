using SharpDX;
using System;

namespace Refterm
{
    public class SourceBuffer
    {
        public SourceBuffer(int dataSize)
        {
            this.backingBuffer = new char[dataSize];
            Data = backingBuffer;
            DataSize = dataSize;
        }

        public int DataSize { get; private set; }

        /// <summary>
        /// char*
        /// </summary>
        public Memory<char> Data { get; private set; }

        // NOTE(casey): For circular buffer
        public int RelativePoint { get; set; }

        // NOTE(casey): For cache checking
        public ulong AbsoluteFilledSize { get; set; }

        public char[] backingBuffer { get; private set; }

        internal void Clear()
        {
            AbsoluteFilledSize = 0;
            RelativePoint = 0;

            var len = backingBuffer.Length;

            for (var i = 0; i < len; i++)
            {
                backingBuffer[i] = '\0';
            }
        }

        public void Reset()
        {
            RelativePoint = 0;
        }
    }
}
