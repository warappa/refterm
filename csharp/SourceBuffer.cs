using System;

namespace Refterm
{
    class SourceBuffer
    {
        public int DataSize;


        /// <summary>
        /// char*
        /// </summary>
        public Memory<char> Data;

        // NOTE(casey): For circular buffer
        public int RelativePoint;

        // NOTE(casey): For cache checking
        public int AbsoluteFilledSize;

        public char[] InternalData { get; internal set; }

        internal void Clear()
        {
            AbsoluteFilledSize = 0;
            RelativePoint = 0;

            for (var i = 0; i < InternalData.Length; i++)
            {
                InternalData[i] = '\0';
            }
        }
    }
}
